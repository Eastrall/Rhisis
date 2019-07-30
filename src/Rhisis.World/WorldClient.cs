using Ether.Network.Common;
using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Packets;
using Rhisis.World.Client;
using Rhisis.World.Game.Entities;
using Sylver.HandlerInvoker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World
{
    public sealed class WorldClient : NetUser, IWorldClient
    {
        private ILogger<WorldClient> _logger;
        private IHandlerInvoker _handlerInvoker;

        /// <inheritdoc />
        public uint SessionId { get; }

        /// <inheritdoc />
        public IPlayerEntity Player { get; set; }

        /// <inheritdoc />
        public string RemoteEndPoint { get; private set; }

        /// <inheritdoc />
        public DateTime LoggedInAt { get; set; }

        /// <summary>
        /// Creates a new <see cref="WorldClient"/> instance.
        /// </summary>
        public WorldClient()
        {
            this.SessionId = RandomHelper.GenerateSessionKey();
        }

        /// <summary>
        /// Initialize the client and send welcome packet.
        /// </summary>
        public void Initialize(ILogger<WorldClient> logger, IHandlerInvoker handlerInvoker)
        {
            this._logger = logger;
            this._handlerInvoker = handlerInvoker;
            this.RemoteEndPoint = this.Socket.RemoteEndPoint.ToString();
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (this.Socket == null)
            {
                this._logger.LogTrace("Skip to handle world packet from {this.RemoteEndPoint}. Reason: client is not connected.");
                return;
            }

            try
            {
                packet.Read<uint>(); // DPID: Always 0xFFFFFFFF (uint.MaxValue)
                packetHeaderNumber = packet.Read<uint>();
#if DEBUG
                this._logger.LogTrace("Received {0} packet from {1}.", (PacketType)packetHeaderNumber, this.RemoteEndPoint);
#endif
                this._handlerInvoker.Invoke((PacketType)packetHeaderNumber, this, packet);
            }
            catch (ArgumentNullException)
            {
                if (Enum.IsDefined(typeof(PacketType), packetHeaderNumber))
                    this._logger.LogWarning("Received an unimplemented World packet {0} (0x{1}) from {2}.", Enum.GetName(typeof(PacketType), packetHeaderNumber), packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
                else
                    this._logger.LogWarning("[SECURITY] Received an unknown World packet 0x{0} from {1}.", packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"An error occured while handling a world packet.");
                this._logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }

        /// <summary>
        /// Save the entity to the database.
        /// </summary>
        private void Save()
        {
            if (this.Player == null)
                return;

            var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbCharacter character = database.Characters.Get(this.Player.PlayerData.Id);

            if (character != null)
            {
                character.LastConnectionTime = this.LoggedInAt;
                character.PlayTime += (long)(DateTime.UtcNow - this.LoggedInAt).TotalSeconds;

                character.PosX = this.Player.Object.Position.X;
                character.PosY = this.Player.Object.Position.Y;
                character.PosZ = this.Player.Object.Position.Z;
                character.Angle = this.Player.Object.Angle;
                character.MapId = this.Player.Object.MapId;
                character.MapLayerId = this.Player.Object.LayerId;
                character.Gender = this.Player.VisualAppearance.Gender;
                character.HairColor = this.Player.VisualAppearance.HairColor;
                character.HairId = this.Player.VisualAppearance.HairId;
                character.FaceId = this.Player.VisualAppearance.FaceId;
                character.SkinSetId = this.Player.VisualAppearance.SkinSetId;
                character.Level = this.Player.Object.Level;

                character.Gold = this.Player.PlayerData.Gold;
                character.Experience = this.Player.PlayerData.Experience;

                character.Strength = this.Player.Attributes[DefineAttributes.STR];
                character.Stamina = this.Player.Attributes[DefineAttributes.STA];
                character.Dexterity = this.Player.Attributes[DefineAttributes.DEX];
                character.Intelligence = this.Player.Attributes[DefineAttributes.INT];
                character.StatPoints = this.Player.Statistics.StatPoints;
                character.SkillPoints = this.Player.Statistics.SkillPoints;

                character.Hp = this.Player.Health.Hp;
                character.Mp = this.Player.Health.Mp;
                character.Fp = this.Player.Health.Fp;

                // Delete items
                var itemsToDelete = new List<DbItem>(character.Items.Count);
                itemsToDelete.AddRange(from dbItem in character.Items
                                       let inventoryItem = this.Player.Inventory.GetItem(x => x.DbId == dbItem.Id) ??
                                                           new Game.Structures.Item()
                                       where inventoryItem.Id == -1
                                       select dbItem);
                itemsToDelete.ForEach(x => character.Items.Remove(x));

                // Add or update items
                foreach (var item in this.Player.Inventory.Items)
                {
                    if (item.Id == -1)
                    {
                        continue;
                    }

                    DbItem dbItem = character.Items.FirstOrDefault(x => x.Id == item.DbId);

                    if (dbItem != null)
                    {
                        dbItem.CharacterId = this.Player.PlayerData.Id;
                        dbItem.ItemId = item.Id;
                        dbItem.ItemCount = item.Quantity;
                        dbItem.ItemSlot = item.Slot;
                        dbItem.Refine = item.Refine;
                        dbItem.Element = item.Element;
                        dbItem.ElementRefine = item.ElementRefine;
                        database.Items.Update(dbItem);
                    }
                    else
                    {
                        dbItem = new DbItem
                        {
                            CharacterId = this.Player.PlayerData.Id,
                            CreatorId = item.CreatorId,
                            ItemId = item.Id,
                            ItemCount = item.Quantity,
                            ItemSlot = item.Slot,
                            Refine = item.Refine,
                            Element = item.Element,
                            ElementRefine = item.ElementRefine
                        };

                        database.Items.Create(dbItem);
                    }
                }

                // Taskbar
                character.TaskbarShortcuts.Clear();

                foreach (var applet in Player.Taskbar.Applets.Shortcuts)
                {
                    if (applet == null)
                        continue;

                    var dbApplet = new DbShortcut(ShortcutTaskbarTarget.Applet, applet.SlotIndex, applet.Type,
                        applet.ObjId, applet.ObjectType, applet.ObjIndex, applet.UserId, applet.ObjData, applet.Text);

                    if (applet.Type == ShortcutType.Item)
                    {
                        var item = this.Player.Inventory.GetItem((int)applet.ObjId);
                        dbApplet.ObjectId = (uint)item.Slot;
                    }

                    character.TaskbarShortcuts.Add(dbApplet);
                }


                for (int slotLevel = 0; slotLevel < Player.Taskbar.Items.Shortcuts.Count; slotLevel++)
                {
                    for (int slot = 0; slot < Player.Taskbar.Items.Shortcuts[slotLevel].Count; slot++)
                    {
                        var itemShortcut = Player.Taskbar.Items.Shortcuts[slotLevel][slot];
                        if (itemShortcut == null)
                            continue;

                        var dbItem = new DbShortcut(ShortcutTaskbarTarget.Item, slotLevel, itemShortcut.SlotIndex,
                            itemShortcut.Type, itemShortcut.ObjId, itemShortcut.ObjectType, itemShortcut.ObjIndex,
                            itemShortcut.UserId, itemShortcut.ObjData, itemShortcut.Text);

                        if (itemShortcut.Type == ShortcutType.Item)
                        {
                            var item = this.Player.Inventory.GetItem((int)itemShortcut.ObjId);
                            dbItem.ObjectId = (uint)item.Slot;
                        }

                        character.TaskbarShortcuts.Add(dbItem);
                    }
                }

                foreach (var queueItem in Player.Taskbar.Queue.Shortcuts)
                {
                    if (queueItem == null)
                        continue;

                    character.TaskbarShortcuts.Add(new DbShortcut(ShortcutTaskbarTarget.Queue, queueItem.SlotIndex,
                        queueItem.Type, queueItem.ObjId, queueItem.ObjectType, queueItem.ObjIndex, queueItem.UserId,
                        queueItem.ObjData, queueItem.Text));
                }
            }

            database.Complete();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Save();
                this.Player?.Delete();
            }

            base.Dispose(disposing);
        }
    }
}

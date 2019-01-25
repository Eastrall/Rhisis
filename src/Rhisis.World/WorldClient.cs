using Ether.Network.Common;
using Ether.Network.Packets;
using NLog;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Exceptions;
using Rhisis.Core.Helpers;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Database.Repositories;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using Rhisis.World.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World
{
    public sealed class WorldClient : NetUser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the ID assigned to this session.
        /// </summary>
        public uint SessionId { get; }

        /// <summary>
        /// Gets or sets the player entity.
        /// </summary>
        public IPlayerEntity Player { get; set; }

        /// <summary>
        /// Gets the world server's instance.
        /// </summary>
        public IWorldServer WorldServer { get; private set; }

        /// <summary>
        /// Gets the remote end point (IP and port) for this client.
        /// </summary>
        public string RemoteEndPoint { get; private set; }

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
        public void InitializeClient(IWorldServer server)
        {
            this.WorldServer = server;
            this.RemoteEndPoint = this.Socket.RemoteEndPoint.ToString();
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (Socket == null)
            {
                Logger.Trace("Skip to handle packet from {0}. Reason: client is no more connected.", this.RemoteEndPoint);
                return;
            }

            try
            {
                packet.Read<uint>(); // DPID: Always 0xFFFFFFFF (uint.MaxValue)
                packetHeaderNumber = packet.Read<uint>();

                bool packetInvokSuccess = PacketHandler<WorldClient>.Invoke(this, packet as FFPacket, (PacketType)packetHeaderNumber);

                if (!packetInvokSuccess)
                {
                    if (Enum.IsDefined(typeof(PacketType), packetHeaderNumber))
                        Logger.Warn("Received an unimplemented World packet {0} (0x{1}) from {2}.", Enum.GetName(typeof(PacketType), packetHeaderNumber), packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
                    else
                        Logger.Warn("[SECURITY] Received an unknown World packet 0x{0} from {1}.", packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
                }
            }
            catch (RhisisPacketException packetException)
            {
                Logger.Error("Packet handle error from {0}. {1}", this.RemoteEndPoint, packetException);
                Logger.Debug(packetException.InnerException?.StackTrace);
            }
        }

        /// <summary>
        /// Save the entity to the database.
        /// </summary>
        private void Save()
        {
            if (this.Player == null)
                return;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                DbCharacter character = database.Characters.Get(this.Player.PlayerData.Id);

                if (character != null)
                {
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

                    character.Strength = this.Player.Statistics.Strength;
                    character.Stamina = this.Player.Statistics.Stamina;
                    character.Dexterity = this.Player.Statistics.Dexterity;
                    character.Intelligence = this.Player.Statistics.Intelligence;
                    character.StatPoints = this.Player.Statistics.StatPoints;

                    // Delete items
                    var itemsToDelete = new List<DbItem>(character.Items.Count);
                    itemsToDelete.AddRange(from dbItem
                        in character.Items
                        let inventoryItem = this.Player.Inventory.GetItem(x => x.DbId == dbItem.Id) ?? new Game.Structures.Item()
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
                }

                database.Complete();
            }
        }

        /// <summary>
        /// Despawns the current player and notify other players arround.
        /// </summary>
        /// <param name="currentMap"></param>
        private void DespawnPlayer(IMapInstance currentMap)
        {
            IEnumerable<IEntity> entitiesAround = from x in currentMap.Entities
                                                  where this.Player.Object.Position.IsInCircle(x.Object.Position, VisibilitySystem.VisibilityRange) && x != this.Player
                                                  select x;

            this.Player.Object.Spawned = false;

            foreach (IEntity entity in entitiesAround)
            {
                if (entity.Type == WorldEntityType.Player)
                    WorldPacketFactory.SendDespawnObjectTo(entity as IPlayerEntity, this.Player);

                entity.Object.Entities.Remove(this.Player);
            }

            currentMap.DeleteEntity(this.Player);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Player != null)
                    this.DespawnPlayer(this.Player.Object.CurrentMap);

                this.Save();
            }

            base.Dispose(disposing);
        }
    }
}

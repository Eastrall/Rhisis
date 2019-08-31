using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.PlayerData
{
    [Injectable]
    public sealed class PlayerDataSystem : IPlayerDataSystem
    {
        private readonly IDatabase _database;
        private readonly IMoverPacketFactory _moverPacketFactory;
        private readonly ITextPacketFactory _textPacketFactory;

        /// <summary>
        /// Creates a new <see cref="PlayerDataSystem"/> instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="moverPacketFactory">Mover packet factory.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public PlayerDataSystem(IDatabase database, IMoverPacketFactory moverPacketFactory, ITextPacketFactory textPacketFactory)
        {
            this._database = database;
            this._moverPacketFactory = moverPacketFactory;
            this._textPacketFactory = textPacketFactory;
        }

        /// <inheritdoc />
        public bool IncreaseGold(IPlayerEntity player, int goldAmount)
        {
            long gold = player.PlayerData.Gold + goldAmount;

            if (gold > int.MaxValue || gold < 0) // Check gold overflow
            {
                this._textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_TOOMANYMONEY_USE_PERIN);
                return false;
            }
            else
            {
                player.PlayerData.Gold = (int)gold;
                this._moverPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
            }

            return true;
        }

        /// <inheritdoc />
        public bool DecreaseGold(IPlayerEntity player, int goldAmount)
        {
            player.PlayerData.Gold = Math.Max(player.PlayerData.Gold - goldAmount, 0);

            this._moverPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);

            return true;
        }

        /// <inheritdoc />
        public void SavePlayer(IPlayerEntity player)
        {
            if (player == null)
                return;

            DbCharacter character = this._database.Characters.Get(player.PlayerData.Id);

            if (character != null)
            {
                character.LastConnectionTime = player.PlayerData.LoggedInAt;
                character.PlayTime += (long)(DateTime.UtcNow - player.PlayerData.LoggedInAt).TotalSeconds;

                character.PosX = player.Object.Position.X;
                character.PosY = player.Object.Position.Y;
                character.PosZ = player.Object.Position.Z;
                character.Angle = player.Object.Angle;
                character.MapId = player.Object.MapId;
                character.MapLayerId = player.Object.LayerId;
                character.Gender = player.VisualAppearance.Gender;
                character.HairColor = player.VisualAppearance.HairColor;
                character.HairId = player.VisualAppearance.HairId;
                character.FaceId = player.VisualAppearance.FaceId;
                character.SkinSetId = player.VisualAppearance.SkinSetId;
                character.Level = player.Object.Level;

                character.Gold = player.PlayerData.Gold;
                character.Experience = player.PlayerData.Experience;

                character.Strength = player.Attributes[DefineAttributes.STR];
                character.Stamina = player.Attributes[DefineAttributes.STA];
                character.Dexterity = player.Attributes[DefineAttributes.DEX];
                character.Intelligence = player.Attributes[DefineAttributes.INT];
                character.StatPoints = player.Statistics.StatPoints;
                character.SkillPoints = player.Statistics.SkillPoints;

                character.Hp = player.Health.Hp;
                character.Mp = player.Health.Mp;
                character.Fp = player.Health.Fp;

                // Delete items
                var itemsToDelete = new List<DbItem>(character.Items.Count);
                itemsToDelete.AddRange(from dbItem in character.Items
                                       let inventoryItem = player.Inventory.GetItem(x => x.DbId == dbItem.Id) ??
                                                           new Game.Structures.Item()
                                       where inventoryItem.Id == -1
                                       select dbItem);
                itemsToDelete.ForEach(x => character.Items.Remove(x));

                // Add or update items
                foreach (var item in player.Inventory.Items)
                {
                    if (item.Id == -1)
                    {
                        continue;
                    }

                    DbItem dbItem = character.Items.FirstOrDefault(x => x.Id == item.DbId);

                    if (dbItem != null)
                    {
                        dbItem.CharacterId = player.PlayerData.Id;
                        dbItem.ItemId = item.Id;
                        dbItem.ItemCount = item.Quantity;
                        dbItem.ItemSlot = item.Slot;
                        dbItem.Refine = item.Refine;
                        dbItem.Element = item.Element;
                        dbItem.ElementRefine = item.ElementRefine;
                        this._database.Items.Update(dbItem);
                    }
                    else
                    {
                        dbItem = new DbItem
                        {
                            CharacterId = player.PlayerData.Id,
                            CreatorId = item.CreatorId,
                            ItemId = item.Id,
                            ItemCount = item.Quantity,
                            ItemSlot = item.Slot,
                            Refine = item.Refine,
                            Element = item.Element,
                            ElementRefine = item.ElementRefine
                        };

                        this._database.Items.Create(dbItem);
                    }
                }

                // Taskbar
                character.TaskbarShortcuts.Clear();

                foreach (var applet in player.Taskbar.Applets.Objects)
                {
                    if (applet == null)
                        continue;

                    var dbApplet = new DbShortcut(ShortcutTaskbarTarget.Applet, applet.SlotIndex, applet.Type,
                        applet.ObjectId, applet.ObjectType, applet.ObjectIndex, applet.UserId, applet.ObjectData, applet.Text);

                    if (applet.Type == ShortcutType.Item)
                    {
                        var item = player.Inventory.GetItem((int)applet.ObjectId);
                        dbApplet.ObjectId = (uint)item.Slot;
                    }

                    character.TaskbarShortcuts.Add(dbApplet);
                }


                for (int slotLevel = 0; slotLevel < player.Taskbar.Items.Objects.Count; slotLevel++)
                {
                    for (int slot = 0; slot < player.Taskbar.Items.Objects[slotLevel].Count; slot++)
                    {
                        var itemShortcut = player.Taskbar.Items.Objects[slotLevel][slot];
                        if (itemShortcut == null)
                            continue;

                        var dbItem = new DbShortcut(ShortcutTaskbarTarget.Item, slotLevel, itemShortcut.SlotIndex,
                            itemShortcut.Type, itemShortcut.ObjectId, itemShortcut.ObjectType, itemShortcut.ObjectIndex,
                            itemShortcut.UserId, itemShortcut.ObjectData, itemShortcut.Text);

                        if (itemShortcut.Type == ShortcutType.Item)
                        {
                            var item = player.Inventory.GetItem((int)itemShortcut.ObjectId);
                            dbItem.ObjectId = (uint)item.Slot;
                        }

                        character.TaskbarShortcuts.Add(dbItem);
                    }
                }

                foreach (var queueItem in player.Taskbar.Queue.Shortcuts)
                {
                    if (queueItem == null)
                        continue;

                    character.TaskbarShortcuts.Add(new DbShortcut(ShortcutTaskbarTarget.Queue, queueItem.SlotIndex,
                        queueItem.Type, queueItem.ObjectId, queueItem.ObjectType, queueItem.ObjectIndex, queueItem.UserId,
                        queueItem.ObjectData, queueItem.Text));
                }
            }

            this._database.Complete();
        }
    }
}

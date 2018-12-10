using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.Extensions;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory.EventArgs;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Inventory
{
    [System(SystemType.Notifiable)]
    public class InventorySystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static readonly int RightWeaponSlot = 52;
        public static readonly int EquipOffset = 42;
        public static readonly int MaxItems = 73;
        public static readonly int InventorySize = EquipOffset;
        public static readonly int MaxHumanParts = MaxItems - EquipOffset;
        public static readonly Item Hand = new Item(11, 1, -1, RightWeaponSlot);

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;
            
            if (!e.CheckArguments())
            {
                Logger.Error("Cannot execute inventory action: {0} due to invalid arguments.", e.GetType());
                return;
            }

            switch (e)
            {
                case InventoryInitializeEventArgs inventoryInitializeEvent:
                    this.InitializeInventory(playerEntity, inventoryInitializeEvent);
                    break;
                case InventoryMoveEventArgs inventoryMoveEvent:
                    this.MoveItem(playerEntity, inventoryMoveEvent);
                    break;
                case InventoryEquipEventArgs inventoryEquipEvent:
                    this.EquipItem(playerEntity, inventoryEquipEvent);
                    break;
                case InventoryCreateItemEventArgs inventoryCreateItemEventArgs:
                    this.CreateItem(playerEntity, inventoryCreateItemEventArgs);
                    break;
                default:
                    Logger.Warn("Unknown inventory action type: {0} for player {1}", e.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// Initialize the player's inventory.
        /// </summary>
        /// <param name="player">Current player</param>
        /// <param name="e"></param>
        private void InitializeInventory(IPlayerEntity player, InventoryInitializeEventArgs e)
        {
            player.Inventory = new ItemContainerComponent(MaxItems, InventorySize);
            var inventory = player.Inventory;

            if (e.Items != null)
            {
                foreach (Database.Entities.DbItem item in e.Items)
                {
                    int uniqueId = inventory.Items[item.ItemSlot].UniqueId;

                    inventory.Items[item.ItemSlot] = new Item(item)
                    {
                        UniqueId = uniqueId,
                    };
                }
            }

            for (int i = EquipOffset; i < MaxItems; ++i)
            {
                if (inventory.Items[i].Id == -1)
                    inventory.Items[i].UniqueId = -1;
            }
        }

        /// <summary>
        /// Move an item.
        /// </summary>
        /// <param name="player"></param>
        private void MoveItem(IPlayerEntity player, InventoryMoveEventArgs e)
        {
            var sourceSlot = e.SourceSlot;
            var destinationSlot = e.DestinationSlot;
            List<Item> items = player.Inventory.Items;

            if (sourceSlot >= MaxItems || destinationSlot >= MaxItems)
                return;

            if (items[sourceSlot].Id == -1 || items[sourceSlot].UniqueId == -1 || items[destinationSlot].UniqueId == -1)
                return;
            
            Item sourceItem = items[sourceSlot];
            Item destItem = items[destinationSlot];
            
            Logger.Debug("Moving item from {0} to {1}", sourceSlot, destinationSlot);

            if (sourceItem.Id == destItem.Id && sourceItem.Data.IsStackable)
            {
                // TODO: stack items
            }
            else
            {
                sourceItem.Slot = destinationSlot;

                if (destItem.Slot != -1)
                    destItem.Slot = sourceSlot;

                items.Swap(sourceSlot, destinationSlot);
                WorldPacketFactory.SendItemMove(player, sourceSlot, destinationSlot);
            }
        }

        /// <summary>
        /// Equips an item.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void EquipItem(IPlayerEntity player, InventoryEquipEventArgs e)
        {
            var uniqueId = e.UniqueId;
            var part = e.Part;

            Logger.Debug("UniqueId: {0} | Part: {1}", uniqueId, part);

            Item item = player.Inventory.GetItem(uniqueId);
            if (item == null)
                return;

            bool equip = part == -1;

            Logger.Debug("Equip item: {0}", equip.ToString());

            if (equip)
            {
                Logger.Debug("Item: {0}", item.ToString());

                // TODO: check if the player fits the item requirements
                if (item.Data.ItemKind1 == ItemKind1.ARMOR && item.Data.ItemSex != player.VisualAppearance.Gender)
                {
                    Logger.Debug("Wrong sex for armor");
                    // TODO: Send invalid sex error
                    return;
                }

                if (player.Object.Level < item.Data.LimitLevel)
                {
                    Logger.Debug("Player level to low");
                    // TODO: Send low level error
                    return;
                }

                // TODO: SPECIAL: double weapon for blades...

                int sourceSlot = item.Slot;
                int equipedItemSlot = item.Data.Parts + EquipOffset;
                Item equipedItem = player.Inventory[equipedItemSlot];
                
                if (equipedItem != null && equipedItem.Slot != -1)
                {
                    this.UnequipItem(player, equipedItem);
                }

                // Move item
                item.Slot = equipedItemSlot;
                player.Inventory.Items.Swap(sourceSlot, equipedItemSlot);

                WorldPacketFactory.SendItemEquip(player, item, item.Data.Parts, true);
            }
            else
            {
                this.UnequipItem(player, item);
            }
        }

        /// <summary>
        /// Unequip an item.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="item">Item to unequip</param>
        private void UnequipItem(IPlayerEntity player, Item item)
        {
            int sourceSlot = item.Slot;
            int availableSlot = player.Inventory.GetAvailableSlot();
            Logger.Debug("Available slot: {0}", availableSlot);

            if (availableSlot < 0)
            {
                Logger.Debug("No available slots.");
                WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }

            if (item.Id > 0 && item.Slot > EquipOffset)
            {
                int parts = Math.Abs(sourceSlot - EquipOffset);

                item.Slot = availableSlot;
                player.Inventory.Items.Swap(sourceSlot, availableSlot);

                WorldPacketFactory.SendItemEquip(player, item, parts, false);
            }
        }

        /// <summary>
        /// Create a new item.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="e"></param>
        private void CreateItem(IPlayerEntity player, InventoryCreateItemEventArgs e)
        {
            ItemData itemData = e.ItemData;

            if (itemData.IsStackable)
            {
                // TODO: stackable items
            }
            else
            {
                for (var i = 0; i < e.Quantity; i++)
                {
                    int availableSlot = player.Inventory.GetAvailableSlot();

                    if (availableSlot < 0)
                    {
                        WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                        break;
                    }

                    Logger.Debug("Available slot: {0}", availableSlot);

                    var newItem = new Item(e.ItemId, 1, e.CreatorId)
                    {
                        Slot = availableSlot,
                        UniqueId = availableSlot,
                    };

                    player.Inventory.Items[availableSlot] = newItem;
                    WorldPacketFactory.SendItemCreation(player, newItem);
                }
            }
        }
    }
}

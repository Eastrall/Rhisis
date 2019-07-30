using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Extensions;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;
using Rhisis.World.Systems.Drop.EventArgs;
using Rhisis.World.Systems.Inventory.EventArgs;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Inventory
{
    [System(SystemType.Notifiable)]
    public class InventorySystem : ISystem
    {
        public static readonly int RightWeaponSlot = 52;
        public static readonly int EquipOffset = 42;
        public static readonly int MaxItems = 73;
        public static readonly int InventorySize = EquipOffset;
        public static readonly int MaxHumanParts = MaxItems - EquipOffset;
        public static readonly Item Hand = new Item(11, 1, -1, RightWeaponSlot);

        private readonly ILogger Logger;
        private readonly InventoryItemUsage _itemUsage;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <summary>
        /// Creates a new <see cref="InventorySystem"/> instance.
        /// </summary>
        public InventorySystem()
        {
            this.Logger = DependencyContainer.Instance.Resolve<ILogger<InventorySystem>>();
            this._itemUsage = new InventoryItemUsage(null, null);
        }

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity player))
                return;
            
            if (!e.GetCheckArguments())
            {
                Logger.LogError("Cannot execute inventory action: {0} due to invalid arguments.", e.GetType());
                return;
            }

            if (player.Health.IsDead)
            {
                this.Logger.LogWarning($"Cannot execute inventory action {e.GetType()}. Player '{player.Object.Name}' is dead.");
                return;
            }

            switch (e)
            {
                case InventoryInitializeEventArgs inventoryInitializeEvent:
                    this.InitializeInventory(player, inventoryInitializeEvent);
                    break;
                case InventoryMoveEventArgs inventoryMoveEvent:
                    this.ProcessMoveItem(player, inventoryMoveEvent);
                    break;
                case InventoryEquipEventArgs inventoryEquipEvent:
                    this.ProcessEquipItem(player, inventoryEquipEvent);
                    break;
                case InventoryCreateItemEventArgs inventoryCreateItemEvent:
                    this.ProcessCreateItem(player, inventoryCreateItemEvent);
                    break;
                case InventoryDropItemEventArgs inventoryDropItemEvent:
                    this.ProcessDropItem(player, inventoryDropItemEvent);
                    break;
                case InventoryUseItemEventArgs inventoryUseItemEvent:
                    this.ProcessUseItem(player, inventoryUseItemEvent);
                    break;
                default:
                    Logger.LogWarning("Unknown inventory action type: {0} for player {1}", e.GetType(), entity.Object.Name);
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
        }

        /// <summary>
        /// Move an item.
        /// </summary>
        /// <param name="player"></param>
        private void ProcessMoveItem(IPlayerEntity player, InventoryMoveEventArgs e)
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
        private void ProcessEquipItem(IPlayerEntity player, InventoryEquipEventArgs e)
        {
            Item item = player.Inventory.GetItem(e.UniqueId);

            if (item == null)
            {
                this.Logger.LogWarning($"Cannot find item with unique id: {e.UniqueId} for player {player.Object.Name}.");
                return;
            }

            bool equip = e.Part == -1;

            if (equip)
                this.EquipItem(player, item);
            else
                this.UnequipItem(player, item);
        }

        private void EquipItem(IPlayerEntity player, Item item)
        {
            this.Logger.LogTrace($"{player.Object.Name} want to equip {item.Data.Name}.");

            if (!this._itemUsage.IsItemEquipable(player, item))
            {
                this.Logger.LogTrace($"{player.Object.Name} can not equip {item.Data.Name}.");
                return;
            }

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

        /// <summary>
        /// Unequip an item.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="item">Item to unequip</param>
        private void UnequipItem(IPlayerEntity player, Item item)
        {
            int sourceSlot = item.Slot;
            int availableSlot = player.Inventory.GetAvailableSlot();
            Logger.LogDebug("Unequip: Available slot: {0}", availableSlot);

            if (availableSlot < 0)
            {
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }

            if (item.Id > 0 && item.IsEquipped())
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
        private void ProcessCreateItem(IPlayerEntity player, InventoryCreateItemEventArgs e)
        {
            ItemData itemData = e.ItemData;

            if (itemData.IsStackable)
            {
                int quantity = Math.Min(e.Quantity, itemData.PackMax);

                for (int i = 0; i < EquipOffset; i++)
                {
                    Item inventoryItem = player.Inventory.Items[i];

                    if (inventoryItem == null)
                        continue;

                    if (inventoryItem.Id == e.ItemId)
                    {
                        if (inventoryItem.Quantity + quantity > itemData.PackMax)
                        {
                            inventoryItem.Quantity = itemData.PackMax;
                            quantity -= itemData.PackMax - inventoryItem.Quantity;
                        }
                        else
                        {
                            inventoryItem.Quantity += quantity;
                            quantity = 0;
                        }

                        WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.UniqueId, inventoryItem.Quantity);
                    }
                }

                if (quantity > 0)
                {
                    if (!player.Inventory.HasAvailableSlots())
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    else
                    {
                        var item = new Item(e.ItemId, quantity, -1);

                        if (player.Inventory.CreateItem(item))
                            WorldPacketFactory.SendItemCreation(player, item);
                        else
                        {
                            Logger.LogError("Inventory: Failed to create item.");
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < e.Quantity; i++)
                {
                    int availableSlot = player.Inventory.GetAvailableSlot();

                    if (availableSlot < 0)
                    {
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                        break;
                    }

                    var newItem = new Item(e.ItemId, 1, e.CreatorId)
                    {
                        Slot = availableSlot,
                        UniqueId = player.Inventory.Items[availableSlot].UniqueId,
                        Refine = (byte)Math.Max(e.Refine, 0)
                    };

                    player.Inventory.Items[availableSlot] = newItem;
                    WorldPacketFactory.SendItemCreation(player, newItem);
                }
            }
        }


        /// <summary>
        /// Drops an item from the inventory to the ground.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="e">Drop item event arguments.</param>
        private void ProcessDropItem(IPlayerEntity player, InventoryDropItemEventArgs e)
        {
            Item inventoryItem = player.Inventory.GetItem(e.UniqueItemId);

            if (inventoryItem == null)
            {
                Logger.LogWarning($"{player.Object.Name}: Cannot find item with unique Id: {e.UniqueItemId}");
                return;
            }

            if (inventoryItem.Slot >= EquipOffset)
            {
                Logger.LogWarning($"{player.Object.Name}: Cannot drop an equiped item.");
                return;
            }

            int quantityToDrop = Math.Min(e.Quantity, inventoryItem.Quantity);
            if (quantityToDrop < 0)
            {
                Logger.LogError($"{player.Object.Name} tried to drop a negative quantity.");
                return;
            }

            Item itemToDrop = inventoryItem.Clone();
            itemToDrop.Quantity = quantityToDrop;
            player.NotifySystem<DropSystem>(new DropItemEventArgs(itemToDrop));

            inventoryItem.Quantity -= quantityToDrop;
            if (inventoryItem.Quantity <= 0)
                inventoryItem.Reset();

            WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.UniqueId, inventoryItem.Quantity);
        }

        /// <summary>
        /// Use an item from the player's inventory.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void ProcessUseItem(IPlayerEntity player, InventoryUseItemEventArgs e)
        {
            Item inventoryItem = player.Inventory.GetItem(e.UniqueItemId);

            if (inventoryItem == null)
            {
                this.Logger.LogWarning($"Cannot find item with unique Id: {e.UniqueItemId}");
                return;
            }

            if (e.Part >= MaxHumanParts)
            {
                this.Logger.LogWarning($"Parts cannot be grather than {MaxHumanParts}.");
                return;
            }

            if (e.Part != -1)
            {
                if (!player.Battle.IsFighting)
                    this.EquipItem(player, inventoryItem);
            }
            else
            {
                if (inventoryItem.Data.IsUseable && inventoryItem.Quantity > 0)
                    this._itemUsage.UseItem(player, inventoryItem);
            }
        }
    }
}

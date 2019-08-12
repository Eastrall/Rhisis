using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Extensions;
using Rhisis.Core.Structures.Game;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Inventory
{
    [Injectable(ServiceLifetime.Transient)]
    public sealed class InventorySystem : IInventorySystem
    {
        public const int RightWeaponSlot = 52;
        public const int EquipOffset = 42;
        public const int MaxItems = 73;
        public const int InventorySize = EquipOffset;
        public const int MaxHumanParts = MaxItems - EquipOffset;
        public static readonly Item Hand = new Item(11, 1, -1, RightWeaponSlot);
        private readonly ILogger<InventorySystem> _logger;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryPacketFactory _inventoryPacketFactory;
        private readonly IInventoryItemUsage _inventoryItemUsage;
        private readonly IDropSystem _dropSystem;

        public InventorySystem(ILogger<InventorySystem> logger, IItemFactory itemFactory, IInventoryPacketFactory inventoryPacketFactory, IInventoryItemUsage inventoryItemUsage, IDropSystem dropSystem)
        {
            this._logger = logger;
            this._itemFactory = itemFactory;
            this._inventoryPacketFactory = inventoryPacketFactory;
            this._inventoryItemUsage = inventoryItemUsage;
            this._dropSystem = dropSystem;
        }

        /// <inheritdoc />
        public void InitializeInventory(IPlayerEntity player, IEnumerable<DbItem> items)
        {
            player.Inventory = new ItemContainerComponent(MaxItems, InventorySize);
            var inventory = player.Inventory;

            if (items != null)
            {
                foreach (DbItem item in items)
                {
                    int uniqueId = inventory.Items[item.ItemSlot].UniqueId;

                    inventory.Items[item.ItemSlot] = this._itemFactory.CreateItem(item.ItemId, item.Refine, item.Element, item.ElementRefine, item.CreatorId);
                    inventory.Items[item.ItemSlot].Slot = item.ItemSlot;
                    inventory.Items[item.ItemSlot].UniqueId = uniqueId;
                    inventory.Items[item.ItemSlot].Quantity = item.ItemCount;
                }
            }
        }

        /// <inheritdoc />
        public int CreateItem(IPlayerEntity player, ItemDescriptor item, int quantity, int creatorId = -1)
        {
            int createdAmount = 0;

            if (item.Data.IsStackable)
            {
                for (var i = 0; i < EquipOffset; i++)
                {
                    Item inventoryItem = player.Inventory.Items[i];

                    if (inventoryItem.Id == item.Id)
                    {
                        if (inventoryItem.Quantity + quantity > item.Data.PackMax)
                        {
                            int boughtQuantity = inventoryItem.Data.PackMax - inventoryItem.Quantity;

                            createdAmount = boughtQuantity;
                            quantity -= boughtQuantity;
                            inventoryItem.Quantity = inventoryItem.Data.PackMax;
                        }
                        else
                        {
                            createdAmount = quantity;
                            inventoryItem.Quantity += quantity;
                            quantity = 0;
                        }

                        this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.UniqueId, inventoryItem.Quantity);
                    }
                }

                if (quantity > 0)
                {
                    if (!player.Inventory.HasAvailableSlots())
                    {
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    }
                    else
                    {
                        int availableSlot = player.Inventory.GetAvailableSlot();

                        Item newItem = this._itemFactory.CreateItem(item.Id, item.Refine, item.Element, item.ElementRefine, creatorId);

                        if (newItem == null)
                        {
                            throw new ArgumentNullException(nameof(newItem));
                        }

                        newItem.Quantity = quantity;
                        newItem.UniqueId = player.Inventory[availableSlot].UniqueId;
                        newItem.Slot = availableSlot;
                        player.Inventory[availableSlot] = newItem;

                        this._inventoryPacketFactory.SendItemCreation(player, newItem);

                        createdAmount += quantity;
                    }
                }
            }
            else
            {
                while (quantity > 0)
                {
                    if (!player.Inventory.HasAvailableSlots())
                    {
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                        break;
                    }

                    int availableSlot = player.Inventory.GetAvailableSlot();

                    Item newItem = this._itemFactory.CreateItem(item.Id, item.Refine, item.Element, item.ElementRefine, creatorId);

                    if (newItem == null)
                    {
                        throw new ArgumentNullException(nameof(newItem));
                    }

                    newItem.Quantity = 1;
                    newItem.UniqueId = player.Inventory[availableSlot].UniqueId;
                    newItem.Slot = availableSlot;
                    player.Inventory[availableSlot] = newItem;

                    this._inventoryPacketFactory.SendItemCreation(player, newItem);

                    createdAmount++;
                    quantity--;
                }
            }

            return createdAmount;
        }

        /// <inheritdoc />
        public int DeleteItem(IPlayerEntity player, int itemUniqueId, int quantity)
        {
            if (quantity <= 0)
                return 0;

            Item itemToDelete = player.Inventory.GetItem(itemUniqueId);

            if (itemToDelete == null)
                throw new ArgumentNullException(nameof(itemToDelete), $"Cannot find item with unique id: '{itemUniqueId}' in '{player.Object.Name}''s inventory.");

            return this.DeleteItem(player, itemToDelete, quantity);
        }

        /// <inheritdoc />
        public int DeleteItem(IPlayerEntity player, Item itemToDelete, int quantity)
        {
            int quantityToDelete = Math.Min(itemToDelete.Quantity, quantity);

            itemToDelete.Quantity -= quantityToDelete;

            this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, itemToDelete.UniqueId, itemToDelete.Quantity);

            if (itemToDelete.Quantity <= 0)
                itemToDelete.Reset();

            return quantityToDelete;
        }

        /// <inheritdoc />
        public void MoveItem(IPlayerEntity player, byte sourceSlot, byte destinationSlot, bool sendToPlayer = true)
        {
            if (sourceSlot < 0 || sourceSlot >= MaxItems)
            {
                throw new InvalidOperationException("Source slot is out of inventory range.");
            }

            if (destinationSlot < 0 || destinationSlot >= MaxItems)
            {
                throw new InvalidOperationException("Destination slot is out of inventory range.");
            }

            if (sourceSlot == destinationSlot)
            {
                // Nothing to do when moving an item to the same slot.
                return;
            }

            Item sourceItem = player.Inventory[sourceSlot];
            Item destinationItem = player.Inventory[destinationSlot];

            if (sourceItem.Id == destinationItem.Id && sourceItem.Data.IsStackable)
            {
                int newQuantity = sourceItem.Quantity + destinationItem.Quantity;

                if (newQuantity > destinationItem.Data.PackMax)
                {
                    destinationItem.Quantity = destinationItem.Data.PackMax;
                    sourceItem.Quantity = newQuantity - sourceItem.Data.PackMax;

                    this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, sourceItem.UniqueId, sourceItem.Quantity);
                    this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity);
                }
                else
                {
                    destinationItem.Quantity = newQuantity;
                    this.DeleteItem(player, sourceItem.UniqueId, sourceItem.Quantity);
                    this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity);
                }
            }
            else
            {
                sourceItem.Slot = destinationSlot;

                if (destinationItem.Slot != -1)
                    destinationItem.Slot = sourceSlot;

                player.Inventory.Items.Swap(sourceSlot, destinationSlot);

                if (sendToPlayer)
                    this._inventoryPacketFactory.SendItemMove(player, sourceSlot, destinationSlot);
            }
        }

        /// <inheritdoc />
        public void EquipItem(IPlayerEntity player, int itemUniqueId, int equipPart)
        {
            Item itemToEquip = player.Inventory.GetItem(itemUniqueId);

            if (itemToEquip == null)
            {
                throw new ArgumentNullException(nameof(itemToEquip), $"Cannot find item with unique id: '{itemUniqueId}' in {player.Object.Name} inventory.");
            }

            if (!player.Inventory.HasAvailableSlots())
            {
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }

            bool shouldEquip = !itemToEquip.IsEquipped();

            if (shouldEquip)
            {
                if (this.IsItemEquipable(player, itemToEquip))
                {
                    int sourceSlot = itemToEquip.Slot;
                    int equipSlot = itemToEquip.Data.Parts + EquipOffset;

                    this.MoveItem(player, (byte)sourceSlot, (byte)equipSlot, sendToPlayer: false);
                    this.MoveItem(player, (byte)sourceSlot, (byte)player.Inventory.GetAvailableSlot(), sendToPlayer: false);
                    this._inventoryPacketFactory.SendItemEquip(player, itemToEquip, itemToEquip.Data.Parts, true);
                }
            }
            else
            {
                if (itemToEquip.IsEquipped())
                {
                    int targetPart = Math.Abs(itemToEquip.Slot - EquipOffset);

                    if (equipPart != targetPart)
                    {
                        throw new InvalidOperationException($"Equipement parts doesn't match.");
                    }

                    this.MoveItem(player, (byte)itemToEquip.Slot, (byte)player.Inventory.GetAvailableSlot(), sendToPlayer: false);
                    this._inventoryPacketFactory.SendItemEquip(player, itemToEquip, targetPart, false);
                }
            }
        }

        /// <inheritdoc />
        public void UseItem(IPlayerEntity player, int itemUniqueId, int part)
        {
            Item itemToUse = player.Inventory.GetItem(itemUniqueId);

            if (itemToUse == null)
            {
                throw new ArgumentNullException(nameof(itemToUse), $"Cannot find item with unique id: '{itemUniqueId}' in {player.Object.Name} inventory.");
            }

            if (part != -1)
            {
                if (part >= MaxHumanParts)
                {
                    throw new InvalidOperationException($"Invalid equipement part.");
                }

                if (!player.Battle.IsFighting)
                {
                    this.EquipItem(player, itemUniqueId, part);
                }
            }
            else
            {
                if (itemToUse.Data.IsUseable && itemToUse.Quantity > 0)
                {
                    this._logger.LogTrace($"{player.Object.Name} want to use {itemToUse.Data.Name}.");

                    if (player.Inventory.ItemHasCoolTime(itemToUse) && !player.Inventory.CanUseItemWithCoolTime(itemToUse))
                    {
                        this._logger.LogDebug($"Player '{player.Object.Name}' cannot use item {itemToUse.Data.Name}: CoolTime.");
                        return;
                    }

                    switch (itemToUse.Data.ItemKind2)
                    {
                        case ItemKind2.REFRESHER:
                        case ItemKind2.POTION:
                        case ItemKind2.FOOD:
                            this._inventoryItemUsage.UseFoodItem(player, itemToUse);
                            break;
                        case ItemKind2.BLINKWING:
                            this._inventoryItemUsage.UseBlinkwingItem(player, itemToUse);
                            break;
                        default:
                            this._logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            WorldPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                    }
                }
            }
        }

        /// <inhertidoc />
        public void DropItem(IPlayerEntity player, int itemUniqueId, int quantity)
        {
            Item itemToDrop = player.Inventory.GetItem(itemUniqueId);

            if (itemToDrop == null)
            {
                throw new ArgumentNullException(nameof(itemToDrop), $"Cannot find item with unique id: '{itemUniqueId}' in {player.Object.Name} inventory.");
            }

            if (itemToDrop.Slot >= EquipOffset)
            {
                throw new InvalidOperationException($"Cannot drop an equiped item.");
            }

            int quantityToDrop = Math.Min(quantity, itemToDrop.Quantity);

            if (quantityToDrop <= 0)
            {
                throw new InvalidOperationException("Cannot drop a zero or negative quantit.");
            }

            itemToDrop.Quantity = quantityToDrop;
            this._dropSystem.DropItem(player, itemToDrop, owner: null);
            this.DeleteItem(player, itemUniqueId, quantityToDrop);
        }

        /// <summary>
        /// Check if the given item is equipable by a player.
        /// </summary>
        /// <param name="player">Player trying to equip an item.</param>
        /// <param name="item">Item to equip.</param>
        /// <returns>True if the player can equip the item; false otherwise.</returns>
        public bool IsItemEquipable(IPlayerEntity player, Item item)
        {
            if (item.Data.ItemSex != int.MaxValue && item.Data.ItemSex != player.VisualAppearance.Gender)
            {
                this._logger.LogDebug("Wrong sex for armor");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WRONGSEX, item.Data.Name);
                return false;
            }

            if (player.Object.Level < item.Data.LimitLevel)
            {
                this._logger.LogDebug("Player level to low");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REQLEVEL, item.Data.LimitLevel.ToString());
                return false;
            }

            return true;
        }
    }
}

﻿using Microsoft.Extensions.DependencyInjection;
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
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryPacketFactory _inventoryPacketFactory;

        public InventorySystem(IItemFactory itemFactory, IInventoryPacketFactory inventoryPacketFactory)
        {
            this._itemFactory = itemFactory;
            this._inventoryPacketFactory = inventoryPacketFactory;
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

            int quantityToDelete = Math.Min(itemToDelete.Quantity, quantity);

            itemToDelete.Quantity -= quantityToDelete;

            this._inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, itemToDelete.UniqueId, itemToDelete.Quantity);

            if (itemToDelete.Quantity <= 0)
                itemToDelete.Reset();

            return quantityToDelete;
        }

        /// <inheritdoc />
        public void MoveItem(IPlayerEntity player, byte sourceSlot, byte destinationSlot)
        {
            if (sourceSlot < 0 || sourceSlot >= MaxItems)
            {
                throw new InvalidOperationException("Source slot is out of inventory range.");
            }

            if (destinationSlot < 0 || destinationSlot >= MaxItems)
            {
                throw new InvalidOperationException("Destination slot is out of inventory range.");
            }

            Item sourceItem = player.Inventory[sourceSlot];
            Item destinationItem = player.Inventory[destinationSlot];

            if (sourceItem.Id == destinationItem.Id && sourceItem.Data.IsStackable)
            {
                // TODO: stack items
            }
            else
            {
                sourceItem.Slot = destinationSlot;

                if (destinationItem.Slot != -1)
                    destinationItem.Slot = sourceSlot;

                player.Inventory.Items.Swap(sourceSlot, destinationSlot);
                this._inventoryPacketFactory.SendItemMove(player, sourceSlot, destinationSlot);
            }
        }

        public void EquipItem(IPlayerEntity player, int itemUniqueId, int equipPart)
        {
            throw new NotImplementedException();
        }
    }
}

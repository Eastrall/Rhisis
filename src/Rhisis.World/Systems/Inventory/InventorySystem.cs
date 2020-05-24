using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Game;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.Inventory
{
    /// <summary>
    /// Implements the flyff inventory system management.
    /// </summary>
    [Injectable(ServiceLifetime.Transient)]
    public sealed class InventorySystem : IInventorySystem
    {
        public const int LeftWeaponSlot = EquipOffset + (int)ItemPartType.LeftWeapon;
        public const int RightWeaponSlot = EquipOffset + (int)ItemPartType.RightWeapon;
        public const int BulletSlot = EquipOffset + (int)ItemPartType.Bullet;
        public const int EquipOffset = 42;
        public const int MaxItems = 73;
        public const int InventorySize = EquipOffset;
        public const int MaxHumanParts = MaxItems - EquipOffset;
        public static readonly InventoryItem Hand = new InventoryItem(11, 1, -1, RightWeaponSlot);

        private readonly ILogger<InventorySystem> _logger;
        private readonly IRhisisDatabase _database;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryPacketFactory _inventoryPacketFactory;
        private readonly IInventoryItemUsage _inventoryItemUsage;
        private readonly IDropSystem _dropSystem;
        private readonly ITextPacketFactory _textPacketFactory;

        /// <summary>
        /// Gets the initialization order of the inventory system when creating a new player.
        /// </summary>
        public int Order => 0;

        /// <summary>
        /// Creates a new <see cref="InventorySystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="database">Rhisis database.</param>
        /// <param name="itemFactory">Item factory.</param>
        /// <param name="inventoryPacketFactory">Inventory packet factory.</param>
        /// <param name="inventoryItemUsage">Inventory item usage system.</param>
        /// <param name="dropSystem">Drop system.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public InventorySystem(ILogger<InventorySystem> logger, IRhisisDatabase database, IItemFactory itemFactory, IInventoryPacketFactory inventoryPacketFactory, IInventoryItemUsage inventoryItemUsage, IDropSystem dropSystem, ITextPacketFactory textPacketFactory)
        {
            _logger = logger;
            _database = database;
            _itemFactory = itemFactory;
            _inventoryPacketFactory = inventoryPacketFactory;
            _inventoryItemUsage = inventoryItemUsage;
            _dropSystem = dropSystem;
            _textPacketFactory = textPacketFactory;
        }

        /// <inheritdoc />
        public void Initialize(IPlayerEntity player)
        {
            IEnumerable<DbInventoryItem> inventoryItems = _database.InventoryItems
                .Include(x => x.Item)
                    .ThenInclude(x => x.ItemAttributes)
                .Where(x => x.CharacterId == player.PlayerData.Id && !x.Item.IsDeleted);

            if (inventoryItems.Any())
            {
                foreach (DbInventoryItem databaseInventoryItem in inventoryItems)
                {
                    InventoryItem item = _itemFactory.CreateInventoryItem(databaseInventoryItem);

                    if (item != null)
                    {
                        player.Inventory.SetItemAtIndex(item, item.Slot);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Save(IPlayerEntity player)
        {
            IQueryable<DbInventoryItem> query = _database.InventoryItems.Include(x => x.Item);

            var t1 = query.Where(x => x.CharacterId == player.PlayerData.Id).ToList();
            IEnumerable<DbInventoryItem> databaseItemsToRemove = from dbInventoryItem in t1
                                                                 where player.Inventory.GetItem(x => x.DbId == dbInventoryItem.Id) == null
                                                                 select dbInventoryItem;

            if (databaseItemsToRemove.Any())
            {
                foreach (DbInventoryItem dbInventoryItem in databaseItemsToRemove)
                {
                    dbInventoryItem.Item.IsDeleted = true;
                    dbInventoryItem.Item.ItemAttributes.Clear();
                    // TODO: audit
                    _database.InventoryItems.Remove(dbInventoryItem);
                }

                _database.SaveChanges();
            }

            IEnumerable<DbInventoryItem> databaseInventoryItems = _database.InventoryItems
                .Include(x => x.Item)
                .ThenInclude(x => x.ItemAttributes)
                .Where(x => x.CharacterId == player.PlayerData.Id && !x.Item.IsDeleted);

            foreach (InventoryItem item in player.Inventory)
            {
                if (item == null || item.Id == -1)
                {
                    continue;
                }

                DbInventoryItem dbInventoryItem = databaseInventoryItems.FirstOrDefault(x => x.Id == item.DbId);

                if (dbInventoryItem != null)
                {
                    dbInventoryItem.Quantity = item.Quantity;
                    dbInventoryItem.Slot = item.Slot;
                    dbInventoryItem.Item.Refine = item.Refine == 0 ? (byte?)null : item.Refine;
                    dbInventoryItem.Item.Element = item.Element == ElementType.None ? (byte?)null : (byte)item.Element;
                    dbInventoryItem.Item.ElementRefine = item.ElementRefine == 0 ? (byte?)null : item.ElementRefine;
                    // TODO: update attributes
                }
                else
                {
                    dbInventoryItem = new DbInventoryItem
                    {
                        CharacterId = player.PlayerData.Id,
                        Quantity = item.Quantity,
                        Slot = item.Slot,
                        Item = new DbItem
                        {
                            IsDeleted = false,
                            ItemId = item.Id,
                            CreatorId = item.CreatorId,
                            Refine = item.Refine == 0 ? (byte?)null : item.Refine,
                            Element = item.Element == ElementType.None ? (byte?)null : (byte)item.Element,
                            ElementRefine = item.ElementRefine == 0 ? (byte?)null : item.ElementRefine,
                            // TODO: add attributes
                        }
                    };

                    _database.InventoryItems.Add(dbInventoryItem);
                }
            }

            _database.SaveChanges();
        }

        /// <inheritdoc />
        public int CreateItem(IPlayerEntity player, ItemBase item, int quantity, int creatorId = -1, bool sendToPlayer = true)
        {
            int createdAmount = 0;

            if (item.Data.IsStackable)
            {
                for (var i = 0; i < InventoryContainerComponent.InventorySize; i++)
                {
                    InventoryItem inventoryItem = player.Inventory.GetItemAtIndex(i);

                    if (inventoryItem?.Id == item.Id)
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

                        if (sendToPlayer)
                        {
                            _inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.UniqueId, inventoryItem.Quantity);
                        }
                    }
                }

                if (quantity > 0)
                {
                    int availableSlot = player.Inventory.GetAvailableSlot();

                    if (availableSlot == -1)
                    {
                        _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    }
                    else
                    {
                        InventoryItem newItem = _itemFactory.CreateInventoryItem(item.Id, item.Refine, item.Element, item.ElementRefine, creatorId);

                        if (newItem == null)
                        {
                            throw new ArgumentNullException(nameof(newItem));
                        }

                        newItem.Quantity = quantity;
                        newItem.Slot = availableSlot;

                        player.Inventory.SetItemAtSlot(newItem, availableSlot);

                        if (sendToPlayer)
                        {
                            _inventoryPacketFactory.SendItemCreation(player, newItem);
                        }

                        createdAmount += quantity;
                    }
                }
            }
            else
            {
                while (quantity > 0)
                {
                    int availableSlot = player.Inventory.GetAvailableSlot();

                    if (availableSlot == -1)
                    {
                        _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                        break;
                    }

                    InventoryItem newItem = _itemFactory.CreateInventoryItem(item.Id, item.Refine, item.Element, item.ElementRefine, creatorId);

                    if (newItem == null)
                    {
                        throw new ArgumentNullException(nameof(newItem));
                    }

                    newItem.Quantity = 1;
                    newItem.Slot = availableSlot;

                    player.Inventory.SetItemAtSlot(newItem, availableSlot);

                    if (sendToPlayer)
                    {
                        _inventoryPacketFactory.SendItemCreation(player, newItem);
                    }

                    createdAmount++;
                    quantity--;
                }
            }

            return createdAmount;
        }

        /// <inheritdoc />
        public int DeleteItem(IPlayerEntity player, int itemUniqueId, int quantity, bool sendToPlayer = true)
        {
            if (quantity <= 0)
            {
                return 0;
            }

            InventoryItem itemToDelete = player.Inventory.GetItemAtIndex(itemUniqueId);

            if (itemToDelete == null)
            {
                throw new ArgumentNullException(nameof(itemToDelete), $"Cannot find item with unique id: '{itemUniqueId}' in '{player.Object.Name}''s inventory.");
            }

            return DeleteItem(player, itemToDelete, quantity, sendToPlayer);
        }

        /// <inheritdoc />
        public int DeleteItem(IPlayerEntity player, InventoryItem itemToDelete, int quantity, bool sendToPlayer = true)
        {
            int quantityToDelete = Math.Min(itemToDelete.Quantity, quantity);

            itemToDelete.Quantity -= quantityToDelete;

            if (sendToPlayer)
            {
                _inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, itemToDelete.UniqueId, itemToDelete.Quantity);
            }

            if (itemToDelete.Quantity <= 0)
            {
                itemToDelete.Reset();
                //player.Inventory.SetItemAtSlot(null, itemToDelete.Slot);
            }

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
                throw new InvalidOperationException("Cannot move an item to the same slot.");
            }

            InventoryItem sourceItem = player.Inventory.GetItemAtSlot(sourceSlot);

            if (sourceItem == null)
            {
                throw new InvalidOperationException("Source item not found");
            }

            InventoryItem destinationItem = player.Inventory.GetItemAtSlot(destinationSlot);

            if (destinationItem != null && sourceItem.Id == destinationItem.Id && sourceItem.Data.IsStackable)
            {
                int newQuantity = sourceItem.Quantity + destinationItem.Quantity;

                if (newQuantity > destinationItem.Data.PackMax)
                {
                    destinationItem.Quantity = destinationItem.Data.PackMax;
                    sourceItem.Quantity = newQuantity - sourceItem.Data.PackMax;

                    _inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, sourceItem.UniqueId, sourceItem.Quantity);
                    _inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity);
                }
                else
                {
                    destinationItem.Quantity = newQuantity;
                    DeleteItem(player, sourceItem.UniqueId, sourceItem.Quantity);
                    _inventoryPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity);
                }
            }
            else
            {
                sourceItem.Slot = destinationSlot;

                if (destinationItem != null && destinationItem.Slot != -1)
                {
                    destinationItem.Slot = sourceSlot;
                }

                player.Inventory.Swap(sourceSlot, destinationSlot);

                if (sendToPlayer)
                {
                    _inventoryPacketFactory.SendItemMove(player, sourceSlot, destinationSlot);
                }
            }
        }

        /// <inheritdoc />
        public bool EquipItem(IPlayerEntity player, int itemUniqueId, int equipPart)
        {
            InventoryItem itemToEquip = player.Inventory.GetItemAtIndex(itemUniqueId);

            if (itemToEquip == null)
            {
                throw new ArgumentNullException(nameof(itemToEquip), $"Cannot find item with unique id: '{itemUniqueId}' in {player.Object.Name} inventory.");
            }

            bool isItemEquiped = player.Inventory.IsItemEquiped(itemToEquip);

            if (isItemEquiped)
            {
                int availableSlot = player.Inventory.GetAvailableSlot();

                if (availableSlot == -1)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    return false;
                }

                MoveItem(player, (byte)itemToEquip.Slot, (byte)availableSlot, sendToPlayer: false);
                _inventoryPacketFactory.SendItemEquip(player, itemToEquip, (int)itemToEquip.Data.Parts, false);

                _logger.LogDebug($"Unequip {itemToEquip} to slot {itemToEquip.Slot}");
            }
            else
            {
                if (!IsItemEquipable(player, itemToEquip))
                {
                    return false;
                }

                InventoryItem equipedItem = player.Inventory.GetEquipedItem(itemToEquip.Data.Parts);

                if (equipedItem != null)
                {
                    _logger.LogDebug($"Unequip {equipedItem} and equip {itemToEquip}");

                    if (!EquipItem(player, equipedItem.UniqueId, (int)equipedItem.Data.Parts))
                    {
                        _logger.LogWarning($"Failed to unequip {equipedItem} to equip {itemToEquip}");
                        return false;
                    }

                }

                int equipIndex = (int)itemToEquip.Data.Parts + InventoryContainerComponent.EquipOffset;

                MoveItem(player, (byte)itemToEquip.Slot, (byte)equipIndex, sendToPlayer: false);
                _inventoryPacketFactory.SendItemEquip(player, itemToEquip, (int)itemToEquip.Data.Parts, true);

                _logger.LogDebug($"Equip {itemToEquip}");
            }

            return true;
        }

        /// <inheritdoc />
        public void UseItem(IPlayerEntity player, int itemUniqueId, int part)
        {
            InventoryItem itemToUse = player.Inventory.GetItemAtIndex(itemUniqueId);

            if (itemToUse == null)
            {
                throw new ArgumentNullException(nameof(itemToUse), $"Cannot find item with unique id: '{itemUniqueId}' in {player.Object.Name} inventory.");
            }

            if (part >= 0)
            {
                if (part >= MaxHumanParts)
                {
                    throw new InvalidOperationException($"Invalid equipement part.");
                }

                if (!player.Battle.IsFighting)
                {
                    EquipItem(player, itemUniqueId, part);
                }
            }
            else
            {
                if (itemToUse.Data.IsUseable && itemToUse.Quantity > 0)
                {
                    _logger.LogTrace($"{player.Object.Name} want to use {itemToUse.Data.Name}.");

                    if (player.Inventory.ItemHasCoolTime(itemToUse) && !player.Inventory.CanUseItemWithCoolTime(itemToUse))
                    {
                        _logger.LogDebug($"Player '{player.Object.Name}' cannot use item {itemToUse.Data.Name}: CoolTime.");
                        return;
                    }

                    switch (itemToUse.Data.ItemKind2)
                    {
                        case ItemKind2.REFRESHER:
                        case ItemKind2.POTION:
                        case ItemKind2.FOOD:
                            _inventoryItemUsage.UseFoodItem(player, itemToUse);
                            break;
                        case ItemKind2.MAGIC:
                            _inventoryItemUsage.UseMagicItem(player, itemToUse);
                            break;
                        case ItemKind2.SYSTEM:
                            UseSystemItem(player, itemToUse);
                            break;
                        case ItemKind2.GMTEXT:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.GENERAL:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.BUFF:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.BUFF2:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.AIRFUEL:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.FURNITURE:
                        case ItemKind2.PAPERING:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.GUILDHOUSE_FURNITURE:
                        case ItemKind2.GUILDHOUSE_NPC:
                        case ItemKind2.GUILDHOUSE_PAPERING:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.GUILDHOUES_COMEBACK:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                        case ItemKind2.BLINKWING:
                            _inventoryItemUsage.UseBlinkwingItem(player, itemToUse);
                            break;
                        default:
                            _logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            _textPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                            break;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void UseSystemItem(IPlayerEntity player, InventoryItem systemItem)
        {
            switch (systemItem.Data.ItemKind3)
            {
                case ItemKind3.SCROLL:
                    UseScrollItem(player, systemItem);
                    break;
                default:
                    _logger.LogDebug($"Item usage for {systemItem.Data.ItemKind3} is not implemented.");
                    _textPacketFactory.SendSnoop(player, $"Item usage for {systemItem.Data.ItemKind3} is not implemented.");
                    break;
            }
        }

        /// <inheritdoc />
        public void UseScrollItem(IPlayerEntity player, InventoryItem scrollItem)
        {
            switch (scrollItem.Data.Id)
            {
                case DefineItem.II_SYS_SYS_SCR_PERIN:
                    _inventoryItemUsage.UsePerin(player, scrollItem);
                    break;
                default:
                    _logger.LogDebug($"Item usage for {scrollItem.Data.Id} is not implemented.");
                    _textPacketFactory.SendSnoop(player, $"Item usage for {scrollItem.Data.Id} is not implemented.");
                    break;
            }
        }

        /// <inhertidoc />
        public void DropItem(IPlayerEntity player, int itemUniqueId, int quantity)
        {
            InventoryItem itemToDrop = player.Inventory.GetItemAtIndex(itemUniqueId);

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

            _dropSystem.DropItem(player, itemToDrop, owner: null, quantity: quantityToDrop);
            DeleteItem(player, itemUniqueId, quantityToDrop);
        }

        /// <summary>
        /// Check if the given item is equipable by a player.
        /// </summary>
        /// <param name="player">Player trying to equip an item.</param>
        /// <param name="item">Item to equip.</param>
        /// <returns>True if the player can equip the item; false otherwise.</returns>
        private bool IsItemEquipable(IPlayerEntity player, InventoryItem item)
        {
            if (item.Data.ItemSex != int.MaxValue && item.Data.ItemSex != player.VisualAppearance.Gender)
            {
                _logger.LogDebug("Wrong sex for armor");
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WRONGSEX, item.Data.Name);
                return false;
            }

            if (player.Object.Level < item.Data.LimitLevel)
            {
                _logger.LogDebug("Player level to low");
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REQLEVEL, item.Data.LimitLevel.ToString());
                return false;
            }

            return true;
        }
    }
}

using Rhisis.Core.Data;
using Rhisis.Core.Extensions;
using Rhisis.Core.IO;
using Rhisis.World.Game.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class InventoryContainerComponent : ItemContainerComponent
    {
        public const int EquipOffset = 42;
        public const int MaxInventoryItems = 73;
        public const int InventorySize = EquipOffset;
        public const int MaxHumanParts = MaxInventoryItems - EquipOffset;
        private const int MaxItemCoolTimes = 3;

        private readonly long[] _itemsCoolTimes = new long[MaxItemCoolTimes];

        /// <summary>
        /// Gets or sets the delayed action id of the current item behing used.
        /// </summary>
        public Guid ItemInUseActionId { get; set; }

        /// <summary>
        /// Creates a new <see cref="InventoryContainerComponent"/> instance.
        /// </summary>
        public InventoryContainerComponent()
            : base(MaxInventoryItems, InventorySize)
        {
        }

        /// <summary>
        /// Gets the equiped items.
        /// </summary>
        /// <returns>Collection of the equiped items.</returns>
        public IEnumerable<InventoryItem> GetEquipedItems()
        {
            return _itemsMask.GetRange(EquipOffset, MaxHumanParts).Select(x => _items.ElementAtOrDefault(x));
        }

        /// <summary>
        /// Gets an equiped item based on a given item part.
        /// </summary>
        /// <param name="equipedItemPart">Item part type.</param>
        /// <returns>Equiped item if there is one; null otherwise.</returns>
        public InventoryItem GetEquipedItem(ItemPartType equipedItemPart)
        {
            int equipedItemSlot = EquipOffset + (int)equipedItemPart;

            if (equipedItemSlot > MaxInventoryItems || equipedItemSlot < EquipOffset)
            {
                return null;
            }

            return GetItemAtSlot(equipedItemSlot);
        }

        /// <summary>
        /// Checks if the given item is equiped.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if the item is equiped; false otherwise.</returns>
        public bool IsItemEquiped(InventoryItem item) => item.Slot > EquipOffset;

        /// <summary>
        /// Gets the item cool time group.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Returns the item cool time group.</returns>
        public int? GetItemCoolTimeGroup(InventoryItem item)
        {
            if (item.Data.CoolTime <= 0)
            {
                return null;
            }

            return item.Data.ItemKind2 switch
            {
                ItemKind2.FOOD => item.Data.ItemKind3 == ItemKind3.PILL ? 1 : 0,
                ItemKind2.SKILL => 2,
                _ => null,
            };
        }

        /// <summary>
        /// Checks if the item has a cooltime.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ItemHasCoolTime(InventoryItem item) => GetItemCoolTimeGroup(item).HasValue;

        /// <summary>
        /// Check if the given item is a cooltime item and can be used.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Returns true if the item with cooltime can be used; false otherwise.</returns>
        public bool CanUseItemWithCoolTime(InventoryItem item)
        {
            int? group = GetItemCoolTimeGroup(item);

            return group.HasValue && _itemsCoolTimes[group.Value] < Time.GetElapsedTime();
        }

        /// <summary>
        /// Sets item cool time.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="cooltime">Cooltime.</param>
        public void SetCoolTime(InventoryItem item, int cooltime)
        {
            int? group = GetItemCoolTimeGroup(item);

            if (group.HasValue)
            {
                _itemsCoolTimes[group.Value] = Time.GetElapsedTime() + cooltime;
            }
        }
    }
}

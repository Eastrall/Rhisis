using Rhisis.Core.Extensions;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Structures;
using Sylver.Network.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class ItemContainerComponent : IPacketSerializer, IEnumerable<InventoryItem>
    {
        protected readonly List<InventoryItem> _items;
        protected readonly int[] _itemsMask;

        /// <summary>
        /// Gets the <see cref="ItemContainerComponent"/> max capacity.
        /// </summary>
        public int MaxCapacity { get; }

        /// <summary>
        /// Gets the <see cref="ItemContainerComponent"/> max storage capacity.
        /// </summary>
        public int MaxStorageCapacity { get; }

        /// <summary>
        /// Gets the extra storage capacity.
        /// </summary>
        public int ExtraCapacity { get; }

        /// <summary>
        /// Creates a new <see cref="ItemContainerComponent"/> instance.
        /// </summary>
        public ItemContainerComponent(int maxCapacity) :
            this(maxCapacity, maxCapacity)
        { }

        /// <summary>
        /// Creates a new <see cref="ItemContainerComponent"/> instance.
        /// </summary>
        public ItemContainerComponent(int maxCapacity, int maxStorageCapacity)
        {
            MaxCapacity = maxCapacity;
            MaxStorageCapacity = maxStorageCapacity;
            ExtraCapacity = MaxCapacity - MaxStorageCapacity;
            _itemsMask = Enumerable.Range(0, MaxCapacity).ToArray();
            _items = new List<InventoryItem>(MaxCapacity);

            for (int i = 0; i < MaxCapacity; i++)
            {
                _items.Add(new InventoryItem
                {
                    Slot = i,
                    UniqueId = i
                });
            }
        }

        /// <summary>
        /// Gets an item matching a given predicate.
        /// </summary>
        /// <param name="predicate">Match predicate.</param>
        /// <returns>Item matching the predicate function; null otherwise.</returns>
        public InventoryItem GetItem(Func<InventoryItem, bool> predicate)
        {
            InventoryItem item = _items.FirstOrDefault(predicate);

            if (item == null)
            {
                return null;
            }

            return item.Id != -1 ? item : null;
        }

        /// <summary>
        /// Gets an item by its id.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <returns>Item if found; null otherwise.</returns>
        public InventoryItem GetItemById(int id) => GetItem(x => x.Id == id);

        /// <summary>
        /// Gets the item at a given slot.
        /// </summary>
        /// <param name="slot">Container slot.</param>
        /// <returns></returns>
        public InventoryItem GetItemAtSlot(int slot)
        {
            if (slot < 0 || slot > MaxCapacity)
            {
                throw new IndexOutOfRangeException();
            }

            InventoryItem item = _items[_itemsMask[slot]];

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"No item found at slot {slot}.");
            }

            return item.Id != -1 ? item : null;
        }

        /// <summary>
        /// Gets the item at a given index (or unique id).
        /// </summary>
        /// <param name="index">Index or Unique Id.</param>
        /// <returns></returns>
        public InventoryItem GetItemAtIndex(int index)
        {
            if (index < 0 || index > MaxCapacity)
            {
                throw new IndexOutOfRangeException();
            }

            InventoryItem item = _items[index];

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"No item found at index {index}.");
            }

            return item.Id != -1 ? item : null;
        }

        /// <summary>
        /// Gets the number of items of the inventory.
        /// </summary>
        /// <returns></returns>
        public int GetItemCount() => _items.Count(x => x.Id != -1);

        /// <summary>
        /// Check if there is available slots in the container.
        /// </summary>
        /// <returns>True if there is available slots; false otherwise.</returns>
        public bool HasAvailableSlots() => GetAvailableSlot() != -1;

        /// <summary>
        /// Get an available slot number.
        /// </summary>
        /// <returns>Slot number if available; -1 otherwise.</returns>
        public int GetAvailableSlot()
        {
            for (int i = 0; i < MaxStorageCapacity; i++)
            {
                if (_items[_itemsMask[i]].Id == -1)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Checks if the given slot is available.
        /// </summary>
        /// <param name="slot">Slot.</param>
        /// <returns>True if the slot is available; false otherwise.</returns>
        public bool IsSlotAvailable(int slot) => GetItemAtSlot(slot) == null;

        /// <summary>
        /// Swap two slots.
        /// </summary>
        /// <param name="sourceSlot">Source slot.</param>
        /// <param name="destinationSlot">Destination slot.</param>
        public void Swap(int sourceSlot, int destinationSlot)
        {
            _itemsMask.Swap(sourceSlot, destinationSlot);
        }

        /// <summary>
        /// Sets an item at a given index.
        /// </summary>
        /// <param name="item">Index to set.</param>
        /// <param name="index">Index.</param>
        public void SetItemAtIndex(InventoryItem item, int index)
        {
            _items[index] = item;

            if (item != null)
            {
                item.UniqueId = index;
            }
        }

        /// <summary>
        /// Sets an item at a given slot.
        /// </summary>
        /// <param name="item">Item to set.</param>
        /// <param name="slot">Slot.</param>
        public void SetItemAtSlot(InventoryItem item, int slot)
        {
            SetItemAtIndex(item, _itemsMask[slot]);
        }

        /// <inheritdoc />
        public void Serialize(INetPacketStream packet)
        {
            foreach (int itemIndex in _itemsMask)
            {
                packet.Write(itemIndex);
            }

            packet.Write((byte)GetItemCount());

            for (int i = 0; i < MaxCapacity; i++)
            {
                InventoryItem item = _items.ElementAt(i);

                if (item.Id != -1)
                {
                    packet.Write((byte)i);
                    item.Serialize(packet);
                }
            }

            for (int i = 0; i < MaxCapacity; i++)
            {
                packet.Write(_items[i].Slot);
            }
        }

        /// <inheritdoc />
        public IEnumerator<InventoryItem> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

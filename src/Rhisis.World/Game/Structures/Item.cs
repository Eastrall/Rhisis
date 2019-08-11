using Ether.Network.Packets;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Systems.Inventory;
using System.Collections.Generic;

namespace Rhisis.World.Game.Structures
{
    /// <summary>
    /// FlyFF item structure.
    /// </summary>
    public class Item : ItemDescriptor
    {
        /// <summary>
        /// Flyff item refine table.
        /// </summary>
        public static readonly IReadOnlyCollection<int> RefineTable = new[] {0, 2, 4, 6, 8, 10, 13, 16, 19, 21, 24};

        /// <summary>
        /// Gets the item unique Id.
        /// </summary>
        public int UniqueId { get; set; }

        /// <summary>
        /// Gets the creator id of the item.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets the item current slot.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Gets the current used item quantity
        /// </summary>
        public int ExtraUsed { get; set; }

        /// <summary>
        /// Creates an empty <see cref="Item"/>.
        /// </summary>
        /// <remarks>
        /// All values set to -1.
        /// </remarks>
        public Item()
            : this(-1, -1, -1, -1, -1)
        {
        }


        /// <summary>
        /// Create an <see cref="Item"/> with an id and a quantity.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Item quantity</param>
        public Item(int id, int quantity)
            : this(id, quantity, -1, -1, -1)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity and creator id.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        public Item(int id, int quantity, int creatorId)
            : this(id, quantity, creatorId, -1, -1)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id and destination slot.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        public Item(int id, int quantity, int creatorId, int slot)
            : this(id, quantity, creatorId, slot, -1)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id and destination slot.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        /// <param name="uniqueId">Item unique id</param>
        public Item(int id, int quantity, int creatorId, int slot, int uniqueId)
            : this(id, quantity, creatorId, slot, uniqueId, 0)
        {
        }
        
        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id, destination slot and refine.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        /// <param name="uniqueId">Item unique id</param>
        /// <param name="refine">Item refine</param>
        public Item(int id, int quantity, int creatorId, int slot, int uniqueId, byte refine)
            : this(id, quantity, creatorId, slot, uniqueId, refine, 0)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id, destination slot, refine and element.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        /// <param name="uniqueId">Item unique id</param>
        /// <param name="refine">Item refine</param>
        /// <param name="element">Item element</param>
        public Item(int id, int quantity, int creatorId, int slot, int uniqueId, byte refine, byte element)
            : this(id, quantity, creatorId, slot, uniqueId, refine, element, 0)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id, destination slot, refine and element.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        /// <param name="uniqueId">Item unique id</param>
        /// <param name="refine">Item refine</param>
        /// <param name="element">Item element</param>
        /// <param name="elementRefine"></param>
        public Item(int id, int quantity, int creatorId, int slot, int uniqueId, byte refine, byte element, byte elementRefine)
            : this(id, quantity, creatorId, slot, uniqueId, refine, element, elementRefine, 0)
        {
        }

        /// <summary>
        /// Create an <see cref="Item"/> with an id, quantity, creator id, destination slot, refine and element.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="quantity">Itme quantity</param>
        /// <param name="creatorId">Id of the character that created the object (for GM)</param>
        /// <param name="slot">Item slot</param>
        /// <param name="uniqueId">Item unique id</param>
        /// <param name="refine">Item refine</param>
        /// <param name="element">Item element</param>
        /// <param name="elementRefine">Item element refine</param>
        /// <param name="extraUsed">Item extra used quantity</param>
        public Item(int id, int quantity, int creatorId, int slot, int uniqueId, byte refine, byte element,
            byte elementRefine, int extraUsed)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.CreatorId = creatorId;
            this.Slot = slot;
            this.UniqueId = uniqueId;
            this.Refine = refine;
            this.Element = element;
            this.ElementRefine = elementRefine;
            // TODO: review this
            //this.Data = GameResources.Instance.Items[this.Id];
            this.ExtraUsed = extraUsed;
        }

        /// <summary>
        /// Creates a new <see cref="Item"/> based on a database item.
        /// </summary>
        /// <param name="dbItem">Database item</param>
        public Item(Database.Entities.DbItem dbItem)
            : this(dbItem.ItemId, dbItem.ItemCount, dbItem.CreatorId, dbItem.ItemSlot, -1, dbItem.Refine,
                dbItem.Element, dbItem.ElementRefine, 0)
        {
            this.DbId = dbItem.Id;
        }

        public Item(int id, byte refine, byte element, byte elementRefine, ItemData itemData, int creatorId)
        {
            this.Id = id;
            this.Quantity = 1;
            this.Refine = refine;
            this.Element = element;
            this.ElementRefine = elementRefine;
            this.Data = itemData;
            this.CreatorId = creatorId;
        }

        /// <summary>
        /// Serialize the item into the packet.
        /// </summary>
        /// <param name="packet"></param>
        public void Serialize(INetPacketStream packet)
        {
            packet.Write(this.UniqueId);
            packet.Write(this.Id);
            
            packet.Write(0); // Serial number

            if (this.Data != null)
                packet.Write(this.Data.Name.Substring(0, this.Data.Name.Length > 31 ? 31 : this.Data.Name.Length));
            else
                packet.Write("Unknown");

            packet.Write((short) this.Quantity);
            packet.Write<byte>(0); // Repair number
            packet.Write(0); // Hp
            packet.Write(0); // Repair
            packet.Write<byte>(0); // flag ?
            packet.Write((int) this.Refine);
            packet.Write(0); // guild id (cloaks?)
            packet.Write(this.Element);
            packet.Write((int) this.ElementRefine);
            packet.Write(0); // m_nResistSMItemId
            packet.Write(0); // Piercing size
            packet.Write(0); // Ultimate piercing size
            packet.Write(0); // Pet vis
            packet.Write(0); // charged
            packet.Write<long>(0); // m_iRandomOptItemId
            packet.Write(0); // m_dwKeepTime
            packet.Write<byte>(0); // pet
            packet.Write(0); // m_bTranformVisPet
        }
        
        /// <summary>
        /// Clones this <see cref="Item"/>.
        /// </summary>
        /// <returns></returns>
        public Item Clone()
        {
            return new Item(this.Id, this.Quantity, this.CreatorId, this.Slot, this.UniqueId, this.Refine, this.Element,
                this.ElementRefine, this.ExtraUsed);
        }

        public bool IsEquipped() => this.Slot > InventorySystem.EquipOffset;

        /// <summary>
        /// Reset the item.
        /// </summary>
        public void Reset()
        {
            this.Id = -1;
            this.DbId = -1;
            this.Quantity = 0;
            this.CreatorId = -1;
            this.Refine = 0;
            this.Element = 0;
            this.ElementRefine = 0;
            this.ExtraUsed = 0;
            this.Slot = -1;
        }

        /// <summary>
        /// Returns the current <see cref="Item"/> on string format
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{this.Data?.Name}";
    }
}
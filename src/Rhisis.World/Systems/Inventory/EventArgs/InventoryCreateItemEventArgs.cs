using System;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Inventory.EventArgs
{
    public class InventoryCreateItemEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the item id to create.
        /// </summary>
        public int ItemId { get; }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the item creator id.
        /// </summary>
        public int CreatorId { get; }

        /// <summary>
        /// Gets or sets the item refine.
        /// </summary>
        public int Refine { get; set; }

        /// <summary>
        /// Gets the data of the item to create.
        /// </summary>
        public ItemData ItemData { get; private set; }

        /// <summary>
        /// Creates a new <see cref="InventoryCreateItemEventArgs" /> instance.
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="creatorId">Item creator id</param>
        public InventoryCreateItemEventArgs(int itemId, int quantity, int creatorId)
            : this(itemId, quantity, creatorId, 0)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InventoryCreateItemEventArgs"/> instance.
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="creatorId">Item creator id</param>
        /// <param name="refine">Item refine.</param>
        public InventoryCreateItemEventArgs(int itemId, int quantity, int creatorId, int refine)
        {
            this.ItemId = itemId;
            this.Quantity = quantity;
            this.CreatorId = creatorId;
            this.Refine = refine;
        }

        /// <inheritdoc />
        public override bool GetCheckArguments()
        {
            return this.ItemId > 0 && this.Quantity > 0 && this.Quantity <= this.ItemData.PackMax;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Create item: {this.ItemId}; Quantity:{this.Quantity}; Creator: {this.CreatorId}";
        }
    }
}

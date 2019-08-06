using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Inventory.EventArgs
{
    internal sealed class InventoryUseItemEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the unique item id to use.
        /// </summary>
        public int UniqueItemId { get; }

        /// <summary>
        /// Gets or sets the part of the item to use.
        /// </summary>
        public int Part { get; }
        
        /// <summary>
        /// Creates a new <see cref="InventoryUseItemEventArgs"/> instance.
        /// </summary>
        /// <param name="uniqueItemId">Unique item id to use.</param>
        /// <param name="part">Item part.</param>
        public InventoryUseItemEventArgs(int uniqueItemId, int part)
        {
            this.UniqueItemId = uniqueItemId;
            this.Part = part;
        }

        /// <inheritdoc />
        public override bool GetCheckArguments()
        {
            return this.UniqueItemId >= 0 && this.UniqueItemId < InventorySystemOld.MaxItems;
        }
    }
}

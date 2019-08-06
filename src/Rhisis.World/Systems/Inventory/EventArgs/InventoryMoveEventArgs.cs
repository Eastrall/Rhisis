using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Inventory.EventArgs
{
    internal sealed class InventoryMoveEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the source slot of the item.
        /// </summary>
        public byte SourceSlot { get; }

        /// <summary>
        /// Gets the destination slot of the item.
        /// </summary>
        public byte DestinationSlot { get; }

        /// <summary>
        /// Creates a new <see cref="InventoryMoveEventArgs"/> instance.
        /// </summary>
        /// <param name="sourceSlot">Source slot</param>
        /// <param name="destinationSlot">Destination slot</param>
        public InventoryMoveEventArgs(byte sourceSlot, byte destinationSlot)
        {
            SourceSlot = sourceSlot;
            DestinationSlot = destinationSlot;
        }

        /// <inheritdoc />
        public override bool GetCheckArguments()
        {
            return SourceSlot < InventorySystemOld.EquipOffset &&
               DestinationSlot < InventorySystemOld.EquipOffset;
        }
    }
}

using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Inventory.EventArgs
{
    internal sealed class InventoryEquipEventArgs : SystemEventArgs
    {
        public int UniqueId { get; }

        public int Part { get; }

        public InventoryEquipEventArgs(int uniqueId, int part)
        {
            UniqueId = uniqueId;
            Part = part;
        }

        /// <inheritdoc />
        public override bool GetCheckArguments()
        {
            return Part < InventorySystemOld.MaxHumanParts;
        }
    }
}

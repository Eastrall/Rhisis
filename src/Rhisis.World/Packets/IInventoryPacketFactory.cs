using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets
{
    public interface IInventoryPacketFactory
    {
        /// <summary>
        /// Sends a packet for the item creation.
        /// </summary>
        /// <param name="entity">Player entity.</param>
        /// <param name="item">Item to create.</param>
        void SendItemCreation(IPlayerEntity entity, InventoryItem item);

        /// <summary>
        /// Sends a packet that updates an item.
        /// </summary>
        /// <param name="entity">Player entity.</param>
        /// <param name="updateType">Update type.</param>
        /// <param name="uniqueId">Item unique id.</param>
        /// <param name="value">New value.s</param>
        void SendItemUpdate(IPlayerEntity entity, UpdateItemType updateType, int uniqueId, int value);

        /// <summary>
        /// Sends a packet that equips or unequips an item.
        /// </summary>
        /// <param name="entity">Player entity.</param>
        /// <param name="item">Item to equip/unequip.</param>
        /// <param name="targetPart">Target part.</param>
        /// <param name="equip">Boolean value that tells if the item should be equiped or unequiped.</param>
        void SendItemEquip(IPlayerEntity entity, InventoryItem item, int targetPart, bool equip);

        /// <summary>
        /// Sends a packet that moves an item from a source slot to a destination slot.
        /// </summary>
        /// <param name="entity">Player entity.</param>
        /// <param name="sourceSlot">Item source slot.</param>
        /// <param name="destinationSlot">Item destination slot.</param>
        void SendItemMove(IPlayerEntity entity, byte sourceSlot, byte destinationSlot);
    }
}

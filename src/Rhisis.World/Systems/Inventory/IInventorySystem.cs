using Rhisis.Core.Structures.Game;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Inventory
{
    public interface IInventorySystem
    {
        /// <summary>
        /// Initialize the player's inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="items">Player's inventory items.</param>
        void InitializeInventory(IPlayerEntity player, IEnumerable<DbItem> items);

        /// <summary>
        /// Creates an item in player's inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="item">Item model to create.</param>
        /// <param name="quantity">Quantity to create.</param>
        /// <param name="creatorId">Id of the character that created the item. Used for GMs and admins.</param>
        /// <returns>Number of items created.</returns>
        int CreateItem(IPlayerEntity player, ItemDescriptor item, int quantity, int creatorId = -1);

        /// <summary>
        /// Deletes an item in player's inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Item's unique id in inventory.</param>
        /// <param name="quantity">Quantity to delete.</param>
        /// <returns>Deleted quantity.</returns>
        int DeleteItem(IPlayerEntity player, int itemUniqueId, int quantity);

        /// <summary>
        /// Moves an item in player's inventory from a source slot to a destination slot.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="sourceSlot">Source slot.</param>
        /// <param name="destinationSlot">Destinatino slot.</param>
        void MoveItem(IPlayerEntity player, byte sourceSlot, byte destinationSlot);

        /// <summary>
        /// Equip or unequip a player equipment item.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Equip player unique id.</param>
        /// <param name="equipPart">Equip part.</param>
        void EquipItem(IPlayerEntity player, int itemUniqueId, int equipPart);
    }
}

using Rhisis.Core.Structures.Game;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
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
        /// Deletes an item in player's inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemToDelete">Item to delete.</param>
        /// <param name="quantity">Quantity to delete.</param>
        /// <returns>Deleted quantity.</returns>
        int DeleteItem(IPlayerEntity player, Item itemToDelete, int quantity);

        /// <summary>
        /// Moves an item in player's inventory from a source slot to a destination slot.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="sourceSlot">Source slot.</param>
        /// <param name="destinationSlot">Destinatino slot.</param>
        /// <param name="sendToPlayer">Sends the packet to inform the player that an item has been moved.</param>
        void MoveItem(IPlayerEntity player, byte sourceSlot, byte destinationSlot, bool sendToPlayer = true);

        /// <summary>
        /// Equip or unequip a player equipment item.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Equip player unique id.</param>
        /// <param name="equipPart">Equip part.</param>
        void EquipItem(IPlayerEntity player, int itemUniqueId, int equipPart);

        /// <summary>
        /// Uses an item from player's inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Unique id of the item to use.</param>
        /// <param name="part">Item part.</param>
        void UseItem(IPlayerEntity player, int itemUniqueId, int part);

        /// <summary>
        /// Drops an item from player's inventory to the ground.
        /// </summary>
        /// <remarks>
        /// Throwing away your stuff in the ground is bad. You should put them in trash. :-)
        /// </remarks>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Item unique id to drop.</param>
        /// <param name="quantity">Quantity to drop.</param>
        void DropItem(IPlayerEntity player, int itemUniqueId, int quantity);
    }
}

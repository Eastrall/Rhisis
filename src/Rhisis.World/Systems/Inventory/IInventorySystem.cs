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
        /// <param name="player">Current player</param>
        /// <param name="items">Player's inventory items.</param>
        void InitializeInventory(IPlayerEntity player, IEnumerable<DbItem> items);
    }
}

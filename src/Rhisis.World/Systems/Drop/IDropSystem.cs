using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Drop
{
    public interface IDropSystem
    {
        /// <summary>
        /// Drops an item to the ground.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="item">Item to drop.</param>
        /// <param name="owner">Owner.</param>
        void DropItem(IWorldEntity entity, ItemDescriptor item, IWorldEntity owner);

        /// <summary>
        /// Drops gold on the group.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="goldAmount">Gold amount to drop.</param>
        /// <param name="owner">Owner.</param>
        void DropGold(IWorldEntity entity, int goldAmount, IWorldEntity owner);
    }
}

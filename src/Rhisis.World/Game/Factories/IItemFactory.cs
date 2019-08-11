using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Factories
{
    public interface IItemFactory
    {
        /// <summary>
        /// Creates a new Item entity on the given context.
        /// </summary>
        /// <param name="currentMapContext">Map context.</param>
        /// <param name="currentMapLayerContext">Map layer context.</param>
        /// <param name="item">Item model.</param>
        /// <returns>New item entity.</returns>
        IItemEntity CreateItemEntity(IMapInstance currentMapContext, IMapLayer currentMapLayerContext, ItemDescriptor item, IWorldEntity owner = null);

        /// <summary>
        /// Creates a new <see cref="Item"/>.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <param name="refine">Item refine.</param>
        /// <param name="element">Item element.</param>
        /// <param name="elementRefine">Item element refine.</param>
        /// <param name="creatorId">Creator id.</param>
        /// <returns>New item.</returns>
        Item CreateItem(int id, byte refine, byte element, byte elementRefine, int creatorId = -1);
    }
}

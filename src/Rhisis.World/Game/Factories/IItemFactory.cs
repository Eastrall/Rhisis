using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using Rhisis.Database.Entities;
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
        /// <param name="owner">Item owner.</param>
        /// <returns>New item entity.</returns>
        IItemEntity CreateItemEntity(IMapInstance currentMapContext, IMapLayer currentMapLayerContext, ItemBase item, IWorldEntity owner = null);

        /// <summary>
        /// Creates a new <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <param name="refine">Item refine.</param>
        /// <param name="element">Item element.</param>
        /// <param name="elementRefine">Item element refine.</param>
        /// <param name="creatorId">Creator id.</param>
        /// <returns>New inventory item.</returns>
        InventoryItem CreateInventoryItem(int id, byte refine, ElementType element, byte elementRefine, int creatorId = -1);

        /// <summary>
        /// Creates a new <see cref="InventoryItem"/> using its item name.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <param name="refine">Item refine.</param>
        /// <param name="element">Item element.</param>
        /// <param name="elementRefine">Item element refine.</param>
        /// <param name="creatorId">Creator id.</param>
        /// <returns>New inventory item.</returns>
        InventoryItem CreateInventoryItem(string name, byte refine, ElementType element, byte elementRefine, int creatorId = -1);

        /// <summary>
        /// Creates a new <see cref="InventoryItem"/> from a <see cref="DbItem"/> instance.
        /// </summary>
        /// <param name="databaseInventoryItem">Database inventory item.</param>
        /// <returns>New inventory item.</returns>
        InventoryItem CreateInventoryItem(DbInventoryItem databaseInventoryItem);
    }
}

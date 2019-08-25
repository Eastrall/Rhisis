using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Structures;
using System;

namespace Rhisis.World.Game.Factories.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class ItemFactory : IItemFactory
    {
        private readonly ILogger<ItemFactory> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGameResources _gameResources;
        private readonly ObjectFactory _itemFactory;
        private readonly ObjectFactory _itemEntityFactory;

        /// <summary>
        /// Creates a new <see cref="ItemFactory"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="gameResources">Game resources.</param>
        public ItemFactory(ILogger<ItemFactory> logger, IServiceProvider serviceProvider, IGameResources gameResources)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._gameResources = gameResources;
            this._itemFactory = ActivatorUtilities.CreateFactory(typeof(Item), new[] { typeof(int), typeof(byte), typeof(byte), typeof(byte), typeof(ItemData), typeof(int) });
            this._itemEntityFactory = ActivatorUtilities.CreateFactory(typeof(ItemEntity), Type.EmptyTypes);
        }

        /// <inheritdoc />
        public Item CreateItem(int id, byte refine, byte element, byte elementRefine, int creatorId = -1)
        {
            if (!this._gameResources.Items.TryGetValue(id, out ItemData itemData))
            {
                this._logger.LogWarning($"Cannot find item data for item id: '{id}'.");
            }

            return this._itemFactory(this._serviceProvider, new object[] { id, refine, element, elementRefine, itemData, creatorId }) as Item;
        }

        /// <inheritdoc />
        public IItemEntity CreateItemEntity(IMapInstance currentMapContext, IMapLayer currentMapLayerContext, ItemDescriptor item, IWorldEntity owner = null)
        {
            var itemEntity = this._itemEntityFactory(this._serviceProvider, null) as IItemEntity;

            itemEntity.Object = new ObjectComponent
            {
                CurrentMap = currentMapContext,
                MapId = currentMapContext.Id,
                LayerId = currentMapLayerContext.Id,
                ModelId = item.Id,
                Name = item.Data.Name,
                Spawned = true,
                Type = WorldObjectType.Item
            };

            itemEntity.Drop = new DropComponent
            {
                Owner = owner,
                Item = this.CreateItem(item.Id, item.Refine, item.Element, item.ElementRefine)
            };

            currentMapLayerContext.AddEntity(itemEntity);

            return itemEntity;
        }
    }
}

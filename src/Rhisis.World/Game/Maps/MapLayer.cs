using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Systems.Visibility;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps
{
    public sealed class MapLayer : MapContext, IMapLayer
    {
        private readonly IVisibilitySystem _visibilitySystem;

        /// <inheritdoc />
        public IMapInstance ParentMap { get; }

        /// <inheritdoc />
        public ICollection<IMapRegion> Regions { get; }

        /// <summary>
        /// Creates a new <see cref="MapLayer"/> instance.
        /// </summary>
        /// <param name="parentMapInstance">Parent map.</param>
        /// <param name="layerId">Layer id.</param>
        /// <param name="visibilitySystem">Visibility system.qvisual</param>
        public MapLayer(IMapInstance parentMapInstance, int layerId, IVisibilitySystem visibilitySystem)
        {
            this.Id = layerId;
            this.ParentMap = parentMapInstance;
            this._visibilitySystem = visibilitySystem;
        }

        /// <inheritdoc />
        public void Update()
        {
            foreach (var entity in this.Entities)
            {
                IWorldEntity layerEntity = entity.Value;

                if (layerEntity is ILivingEntity livingEntity)
                {
                    livingEntity.Behavior?.Update();
                }

                this._visibilitySystem.Execute(layerEntity);
            }
        }

        /// <inheritdoc />
        public override void DeleteEntity(IWorldEntity entityToDelete)
        {
            base.DeleteEntity(entityToDelete);
            this._visibilitySystem.DespawnEntity(entityToDelete);
        }
    }
}

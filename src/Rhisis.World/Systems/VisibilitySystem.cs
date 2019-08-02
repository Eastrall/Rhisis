using Rhisis.Core.Exceptions;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Packets;
using System.Collections.Generic;

namespace Rhisis.World.Systems
{
    [System]
    public class VisibilitySystem : ISystem
    {
        public static readonly float VisibilityRange = 75f;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Mover;

        /// <summary>
        /// Executes the <see cref="VisibilitySystem"/> logic.
        /// </summary>
        /// <param name="entity">Current entity</param>
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!entity.Object.Spawned)
                return;

            var currentMapLayer = entity.Context as IMapLayer;

            if (currentMapLayer == null)
                throw new RhisisSystemException($"Object {entity.Object.Name}:{entity.Id} doesn't belong to a map layer.");

            var currentMap = currentMapLayer.Parent;

            if (currentMap != null)
                UpdateEntitiesVisibility(entity, currentMap.Entities);

            if (currentMapLayer != null)
            {
                UpdateEntitiesVisibility(entity, currentMapLayer.Entities);

                if (entity.Type == WorldEntityType.Player)
                {
                    foreach (var region in currentMapLayer.Regions)
                    {
                        if (!region.IsActive && entity.Object.Position.Intersects(region.GetRectangle(), VisibilityRange))
                        {
                            region.IsActive = true;
                        }

                        if (region.IsActive && region is IMapRespawnRegion respawner)
                        {
                            UpdateEntitiesVisibility(entity, respawner.Entities);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update entities visibility.
        /// </summary>
        /// <param name="entity">Current entity</param>
        /// <param name="entities">Entities</param>
        private void UpdateEntitiesVisibility(IWorldEntity entity, IEnumerable<IWorldEntity> entities)
        {
            foreach (IWorldEntity otherEntity in entities)
            {
                if (entity.Id == otherEntity.Id)
                    continue;

                if (otherEntity.Type == WorldEntityType.Player && entity.Object.LayerId != otherEntity.Object.LayerId)
                    continue;

                bool canSee = entity.Object.Position.IsInCircle(otherEntity.Object.Position, VisibilityRange) && entity != otherEntity;

                if (canSee && otherEntity.Object.Spawned)
                {
                    if (!entity.Object.Entities.Contains(otherEntity))
                        SpawnOtherEntity(entity, otherEntity);
                }
                else
                {
                    if (entity.Object.Entities.Contains(otherEntity))
                        DespawnOtherEntity(entity, otherEntity);
                }
            }
        }

        /// <summary>
        /// Spawn the other entity for the current entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="otherEntity"></param>
        private void SpawnOtherEntity(IWorldEntity entity, IWorldEntity otherEntity)
        {
            entity.Object.Entities.Add(otherEntity);

            if (entity is IPlayerEntity player)
                WorldPacketFactory.SendSpawnObjectTo(player, otherEntity);

            if (otherEntity.Type != WorldEntityType.Player && !otherEntity.Object.Entities.Contains(entity))
                otherEntity.Object.Entities.Add(entity);

            if (otherEntity is IMovableEntity movableEntity &&
                movableEntity.Moves.DestinationPosition != movableEntity.Object.Position)
            {
                WorldPacketFactory.SendDestinationPosition(movableEntity);
            }
        }

        /// <summary>
        /// Despawns the other entity for the current entity.
        /// </summary>
        /// <param name="entity">Current entity</param>
        /// <param name="otherEntity">other entity</param>
        private void DespawnOtherEntity(IWorldEntity entity, IWorldEntity otherEntity)
        {
            if (entity is IPlayerEntity player)
                WorldPacketFactory.SendDespawnObjectTo(player, otherEntity);

            entity.Object.Entities.Remove(otherEntity);
            
            if (otherEntity.Type != WorldEntityType.Player && otherEntity.Object.Entities.Contains(entity))
                otherEntity.Object.Entities.Remove(entity);
        }
    }
}

using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Visibility
{
    [Injectable]
    public sealed class VisibilitySystem : IVisibilitySystem
    {
        public const float VisibilityRange = 75f;
        private readonly ILogger<VisibilitySystem> _logger;
        private readonly IWorldSpawnPacketFactory _worldSpawnPacketFactory;
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="VisibilitySystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="worldSpawnPacketFactory">World spawn packet factory.</param>
        public VisibilitySystem(ILogger<VisibilitySystem> logger, IWorldSpawnPacketFactory worldSpawnPacketFactory, IMoverPacketFactory moverPacketFactory)
        {
            this._logger = logger;
            this._worldSpawnPacketFactory = worldSpawnPacketFactory;
            this._moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public void Execute(IWorldEntity worldEntity)
        {
            if (!worldEntity.Object.Spawned || worldEntity.Type != WorldEntityType.Player)
                return;

            this.UpdateVisibility(worldEntity, worldEntity.Object.CurrentMap.Entities);
            this.UpdateVisibility(worldEntity, worldEntity.Object.CurrentLayer.Entities);
        }

        /// <inheritdoc />
        public void DespawnEntity(IWorldEntity worldEntity)
        {
            foreach (IWorldEntity entity in worldEntity.Object.Entities)
            {
                if (entity.Object.Entities.Contains(worldEntity))
                {
                    if (entity.Type == WorldEntityType.Player)
                    {
                        this._worldSpawnPacketFactory.SendDespawnObjectTo(entity as IPlayerEntity, worldEntity);
                    }

                    entity.Object.Entities.Remove(worldEntity);
                }
            }

            worldEntity.Object.Entities.Clear();
        }

        /// <summary>
        /// Updates the player's visibility.
        /// </summary>
        /// <param name="worldEntity">Current entity.</param>
        /// <param name="entities">Entities</param>
        private void UpdateVisibility(IWorldEntity worldEntity, IReadOnlyDictionary<uint, IWorldEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.Key == worldEntity.Id)
                    continue;

                IWorldEntity otherEntity = entity.Value;

                bool canSee = worldEntity.Object.Position.IsInCircle(otherEntity.Object.Position, VisibilityRange);

                if (canSee && otherEntity.Object.Spawned)
                {
                    if (!worldEntity.Object.Entities.Contains(otherEntity))
                        SpawnOtherEntity(worldEntity, otherEntity);
                }
                else
                {
                    if (worldEntity.Object.Entities.Contains(otherEntity))
                        DespawnOtherEntity(worldEntity, otherEntity);
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
                this._worldSpawnPacketFactory.SendSpawnObjectTo(player, otherEntity);

            if (otherEntity.Type != WorldEntityType.Player && !otherEntity.Object.Entities.Contains(entity))
                otherEntity.Object.Entities.Add(entity);

            if (otherEntity is IMovableEntity movableEntity &&
                movableEntity.Moves.DestinationPosition != movableEntity.Object.Position)
            {
                this._moverPacketFactory.SendDestinationPosition(movableEntity);
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
                this._worldSpawnPacketFactory.SendDespawnObjectTo(player, otherEntity);

            entity.Object.Entities.Remove(otherEntity);

            if (otherEntity.Type != WorldEntityType.Player && otherEntity.Object.Entities.Contains(entity))
                otherEntity.Object.Entities.Remove(entity);
        }
    }
}

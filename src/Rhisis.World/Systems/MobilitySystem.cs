using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    [System]
    public class MobilitySystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player | WorldEntityType.Monster;

        /// <summary>
        /// Executes the <see cref="MobilitySystem"/> logic.
        /// </summary>
        /// <param name="entity">Current entity</param>
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            var movableEntity = entity as IMovableEntity;

            if (movableEntity.MovableComponent.DestinationPosition.IsZero())
                return;
            
            this.Walk(movableEntity);
        }

        /// <summary>
        /// Process the walk algorithm.
        /// </summary>
        /// <param name="entity">Current entity</param>
        private void Walk(IMovableEntity entity)
        {
            if (entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
                return;

            if (entity.Follow.IsFollowing)
            {
                if (entity.Object.Position.IsInCircle(entity.MovableComponent.DestinationPosition, entity.Follow.FollowDistance) &&
                    !entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
                {
                    entity.MovableComponent.DestinationPosition.Reset();
                    entity.Object.MovingFlags = ObjectState.OBJSTA_STAND;

                    if (entity is IPlayerEntity player)
                        player.Behavior.OnArrived(player);
                    else if (entity is IMonsterEntity monster)
                        monster.Behavior.OnArrived(monster);
                    return;
                }
                if (!entity.Object.Position.IsInCircle(entity.Follow.Target.Object.Position, entity.Follow.FollowDistance))
                {
                    entity.MovableComponent.DestinationPosition = entity.Follow.Target.Object.Position.Clone();
                    entity.Object.MovingFlags &= ~ObjectState.OBJSTA_STAND;
                    entity.Object.MovingFlags |= ObjectState.OBJSTA_FMOVE;
                }
            }

            if (entity.Object.Position.IsInCircle(entity.MovableComponent.DestinationPosition, 0.1f))
            {
                entity.MovableComponent.DestinationPosition.Reset();
                entity.Object.MovingFlags = ObjectState.OBJSTA_STAND;

                if (entity is IPlayerEntity player)
                    player.Behavior.OnArrived(player);
                else if (entity is IMonsterEntity monster)
                    monster.Behavior.OnArrived(monster);
            }
            else
            {
                float entitySpeed = entity.MovableComponent.Speed * entity.MovableComponent.SpeedFactor;
                float speed = entitySpeed * (float)entity.Context.GameTime;
                Vector3 distance = entity.MovableComponent.DestinationPosition - entity.Object.Position;

                entity.Object.Position += distance.Normalize() * speed;

                if (entity.Object.Name.Contains("Aibatt") && entity.Follow.IsFollowing)
                {
                    Logger.Debug(entity.Object.Position);
                    //Logger.Debug($"distance to target: {entity.Object.Position.GetDistance2D(entity.Follow.Target.Object.Position)}");
                }
            }
        }
    }
}

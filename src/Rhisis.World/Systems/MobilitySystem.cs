using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using System;

namespace Rhisis.World.Systems
{
    [System]
    public class MobilitySystem : ISystem
    {
        private const float ArrivalRangeRadius = 1f;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player | WorldEntityType.Monster;

        /// <summary>
        /// Executes the <see cref="MobilitySystem"/> logic.
        /// </summary>
        /// <param name="entity">Current entity</param>
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            if (!entity.Object.Spawned)
                return;

            var movableEntity = entity as IMovableEntity;

            if (movableEntity.Moves.NextMoveTime > Time.GetElapsedTime())
                return;

            movableEntity.Moves.NextMoveTime = Time.GetElapsedTime() + 10;

            if (movableEntity.Moves.DestinationPosition.IsZero())
                return;

            if (movableEntity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
                return;

            if (movableEntity.Follow.IsFollowing)
            {
                if (!movableEntity.Object.Position.IsInCircle(movableEntity.Follow.Target.Object.Position, movableEntity.Follow.FollowDistance))
                {
                    movableEntity.Moves.DestinationPosition = movableEntity.Follow.Target.Object.Position.Clone();
                    movableEntity.Object.MovingFlags &= ~ObjectState.OBJSTA_STAND;
                    movableEntity.Object.MovingFlags |= ObjectState.OBJSTA_FMOVE;
                }
                if (movableEntity.Object.Position.IsInCircle(movableEntity.Follow.Target.Object.Position, movableEntity.Follow.FollowDistance) &&
                    !movableEntity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
                {
                    // Arrived
                    movableEntity.Object.MovingFlags = ObjectState.OBJSTA_STAND;
                }
            }

            this.Walk(movableEntity);
        }

        /// <summary>
        /// Process the walk algorithm.
        /// </summary>
        /// <param name="entity">Current entity</param>
        private void Walk(IMovableEntity entity)
        {
            if (entity.Object.Position.IsInCircle(entity.Moves.DestinationPosition, ArrivalRangeRadius))
            {
                entity.Moves.HasArrived = true;
                entity.Moves.DestinationPosition = entity.Object.Position.Clone();
                entity.Object.MovingFlags &= ~ObjectState.OBJSTA_FMOVE;
                entity.Object.MovingFlags |= ObjectState.OBJSTA_STAND;

                if (entity is IMonsterEntity monster)
                    monster.Behavior.OnArrived();
                else if (entity is IPlayerEntity player)
                    player.Behavior.OnArrived();
            }
            else
            {
                entity.Moves.HasArrived = false;
                float entitySpeed = entity.Moves.Speed * entity.Moves.SpeedFactor; // TODO: Add speed bonuses

                if (entity.Object.MotionFlags.HasFlag(StateFlags.OBJSTAF_WALK))
                    entitySpeed /= 5f;
                else if (entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE))
                    entitySpeed /= 4f;

                float distanceX = entity.Moves.DestinationPosition.X - entity.Object.Position.X;
                float distanceZ = entity.Moves.DestinationPosition.Z - entity.Object.Position.Z;
                double distance = Math.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
                
                // Normalize
                double deltaX = distanceX / distance;
                double deltaZ = distanceZ / distance;
                double offsetX = deltaX * entitySpeed;
                double offsetZ = deltaZ * entitySpeed;

                if (Math.Abs(offsetX) > Math.Abs(distanceX))
                    offsetX = distanceX;
                if (Math.Abs(offsetZ) > Math.Abs(distanceZ))
                    offsetZ = distanceZ;

                if (entity.Type == WorldEntityType.Player && entity.Moves.IsMovingWithKeyboard)
                {
                    float angle = entity.Object.Angle;
                    var movementX = (float)(Math.Sin(angle * (Math.PI / 180)) * Math.Sqrt(offsetX * offsetX + offsetZ * offsetZ));
                    var movementZ = (float)(Math.Cos(angle * (Math.PI / 180)) * Math.Sqrt(offsetX * offsetX + offsetZ * offsetZ));

                    if (entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_FMOVE))
                    {
                        entity.Object.Position.X += movementX;
                        entity.Object.Position.Z -= movementZ;
                    }
                    else if (entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE))
                    {
                        entity.Object.Position.X -= movementX;
                        entity.Object.Position.Z += movementZ;
                    }
                }
                else
                {
                    entity.Object.Position.X += (float)offsetX;
                    entity.Object.Position.Z += (float)offsetZ;
                }
            }
        }
    }
}

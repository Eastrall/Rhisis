using NLog;
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
            if (entity.Object.Position.GetDistance2D(entity.MovableComponent.DestinationPosition) < 0.1f)
            {
                entity.MovableComponent.DestinationPosition.Reset();
                // TODO: Call entity OnArrived()
            }
            else
            {
                float entitySpeed = entity.MovableComponent.Speed * entity.MovableComponent.SpeedFactor;
                float speed = entitySpeed * (float)entity.Context.GameTime;
                Vector3 distance = entity.MovableComponent.DestinationPosition - entity.Object.Position;

                entity.Object.Position += distance.Normalize() * speed;
            }
        }
    }
}

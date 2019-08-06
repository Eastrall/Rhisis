using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Mobility
{
    [Injectable]
    public sealed class MobilitySystem : IMobilitySystem
    {
        /// <inheritdoc />
        public void CalculatePosition(IMovableEntity entity)
        {
            if (!entity.Object.Spawned)
                return;

            if (entity.Moves.NextMoveTime > Time.GetElapsedTime())
                return;

            entity.Moves.NextMoveTime = Time.GetElapsedTime() + 10;

            if (entity.Moves.DestinationPosition.IsZero())
                return;

            if (entity.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
                return;

            // TODO
        }
    }
}

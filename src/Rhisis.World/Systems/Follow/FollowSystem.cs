using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Follow
{
    [System(SystemType.Notifiable)]
    public sealed class FollowSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player | WorldEntityType.Monster;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!(entity is IMovableEntity movableEntity) || !e.GetCheckArguments())
            {
                Logger.Error("FollowSystem: Invalid arguments");
                return;
            }

            switch (e)
            {
                case FollowEventArgs followEvent:
                    this.OnFollow(movableEntity, followEvent);
                    break;
            }
        }

        private void OnFollow(IMovableEntity entity, FollowEventArgs e)
        {
            var entityToFollow = entity.FindEntity<IWorldEntity>(e.TargetId);

            if (entityToFollow == null)
            {
                Logger.Error($"Cannot find entity with object id: {e.TargetId} around {entity.Object.Name}");
                return;
            }

            entity.Follow.Target = entityToFollow;
            entity.Moves.DestinationPosition = entityToFollow.Object.Position.Clone();
            entity.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;

            WorldPacketFactory.SendFollowTarget(entity, entityToFollow, e.Distance);
        }
    }
}

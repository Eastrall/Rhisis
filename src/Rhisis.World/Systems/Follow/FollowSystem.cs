using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;

namespace Rhisis.World.Systems.Follow
{
    [Injectable]
    public sealed class FollowSystem : IFollowSystem
    {
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="FollowSystem"/> instance.
        /// </summary>
        /// <param name="moverPacketFactory">Mover packet factory.</param>
        public FollowSystem(IMoverPacketFactory moverPacketFactory)
        {
            this._moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public void Follow(ILivingEntity livingEntity, IWorldEntity targetEntity, float distance = 1f)
        {
            livingEntity.Follow.Target = targetEntity;
            livingEntity.Moves.DestinationPosition = targetEntity.Object.Position.Clone();
            livingEntity.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;

            this._moverPacketFactory.SendFollowTarget(livingEntity, targetEntity, distance);
        }

        /// <inheritdoc />
        public void Follow(ILivingEntity livingEntity, uint targetId, float distance = 1f)
        {
            var entityToFollow = livingEntity.FindEntity<IWorldEntity>(targetId);

            if (entityToFollow == null)
            {
                throw new ArgumentNullException(nameof(entityToFollow), $"Cannot find entity with object id: {targetId} around {livingEntity.Object.Name}");
            }

            this.Follow(livingEntity, entityToFollow, distance);
        }
    }
}

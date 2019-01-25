using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using System;

namespace Rhisis.World.Game.Behaviors
{
    [Behavior(BehaviorType.Player, IsDefault: true)]
    public sealed class DefaultPlayerBehavior : IBehavior<IPlayerEntity>
    {
        /// <inheritdoc />
        public void Update(IPlayerEntity entity)
        {
            //this.UpdateFollowState(entity);
        }

        /// <inheritdoc />
        public void OnArrived(IPlayerEntity entity)
        {
            Console.WriteLine($"{entity.Object.Name} arrived");

            // TODO: if target is drop, then pick up
            if (entity.Follow.Target is IItemEntity dropItem && dropItem.Object.Spawned)
            {
                if (dropItem.Drop.HasOwner && dropItem.Drop.Owner.Id == entity.Id)
                {
                    Console.WriteLine($"{entity.Object.Name} pick up his {dropItem.Drop.Item.Data.Name}.");
                    dropItem.Object.Spawned = false;
                }

                if (!dropItem.Drop.HasOwner)
                {
                    Console.WriteLine($"{entity.Object.Name} pick up {dropItem.Drop.Item.Data.Name}.");
                    dropItem.Object.Spawned = false;
                }
            }
        }

        private void UpdateFollowState(IPlayerEntity entity)
        {
            if (entity.Follow.IsFollowing)
                entity.MovableComponent.DestinationPosition = entity.Follow.Target.Object.Position.Clone();
        }
    }
}

using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle;
using Rhisis.World.Systems.Follow;
using System;

namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Default behavior for all monsters.
    /// </summary>
    [Behavior(BehaviorType.Monster, IsDefault: true)]
    public class DefaultMonsterBehavior : IBehavior<IMonsterEntity>
    {
        private const float MovingRange = 40f;

        /// <inheritdoc />
        public virtual void Update(IMonsterEntity entity)
        {
            if (entity.Battle.IsFighting)
                this.Fight(entity);
            else
                this.UpdateMoves(entity);

            if (entity.Follow.IsFollowing)
                this.Follow(entity);

            this.UpdateArivalState(entity);
        }

        /// <summary>
        /// Update monster moves.
        /// </summary>
        /// <param name="monster"></param>
        private void UpdateMoves(IMonsterEntity monster)
        {
            if (monster.Timers.NextMoveTime <= Time.TimeInSeconds() && monster.MovableComponent.HasArrived)
            {
                this.MoveToPosition(monster, monster.Region.GetRandomPosition());
            }
        }

        /// <summary>
        /// Update monster's arrival state when it arrives at Destination position.
        /// </summary>
        /// <param name="monster"></param>
        private void UpdateArivalState(IMonsterEntity monster)
        {
            // Monster has arrived to its original position after following a player
            if (monster.MovableComponent.HasArrived && monster.MovableComponent.ReturningToOriginalPosition)
            {
                if (monster.MovableComponent.SpeedFactor >= 2)
                {
                    monster.MovableComponent.SpeedFactor = 1f;
                    WorldPacketFactory.SendSpeedFactor(monster, monster.MovableComponent.SpeedFactor);
                }

                monster.MovableComponent.ReturningToOriginalPosition = false;
            }

            // Monster has arrived to destination and is not following any player
            if (monster.MovableComponent.HasArrived && !monster.Follow.IsFollowing)
            {
                monster.MovableComponent.BeginPosition = monster.Object.Position.Clone();
            }
        }

        /// <summary>
        /// Update the follow state.
        /// </summary>
        /// <param name="monster"></param>
        private void Follow(IMonsterEntity monster)
        {
            if (monster.Object.Position.GetDistance2D(monster.MovableComponent.BeginPosition) >= MovingRange || 
                (monster.Follow.Target != null && !monster.Follow.Target.Object.Spawned))
            {
                monster.Follow.Target = null;
                monster.MovableComponent.ReturningToOriginalPosition = true;
                monster.MovableComponent.SpeedFactor = 2.66f;

                WorldPacketFactory.SendSpeedFactor(monster, monster.MovableComponent.SpeedFactor);
                this.MoveToPosition(monster, monster.MovableComponent.BeginPosition);
                return;
            }

            if (monster.Follow.IsFollowing)
            {
                monster.MovableComponent.DestinationPosition = monster.Follow.Target.Object.Position.Clone();
                monster.NotifySystem<FollowSystem>(new FollowEventArgs(monster.Follow.Target.Id, 1f));
            }
        }

        /// <summary>
        /// Moves the monster to a position.
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="destPosition"></param>
        private void MoveToPosition(IMonsterEntity monster, Vector3 destPosition)
        {
            monster.Timers.NextMoveTime = Time.TimeInSeconds() + RandomHelper.LongRandom(8, 20);
            monster.MovableComponent.DestinationPosition = destPosition.Clone();
            monster.Object.Angle = Vector3.AngleBetween(monster.Object.Position, destPosition);

            WorldPacketFactory.SendDestinationPosition(monster);
        }

        /// <summary>
        /// Process the monster's fight.
        /// </summary>
        /// <param name="monster"></param>
        private void Fight(IMonsterEntity monster)
        {
            if (!monster.Battle.Target.Object.Spawned)
                return;

            if (monster.Timers.NextAttackTime <= Time.TimeInMilliseconds())
            {
                monster.Timers.NextAttackTime = (long)(Time.TimeInMilliseconds() + monster.Data.ReAttackDelay);

                var meleeAttack = new MeleeAttackEventArgs(ObjectMessageType.OBJMSG_ATK1, monster.Battle.Target, monster.Data.AttackSpeed);
                monster.NotifySystem<BattleSystem>(meleeAttack);
            }
        }
    }
}

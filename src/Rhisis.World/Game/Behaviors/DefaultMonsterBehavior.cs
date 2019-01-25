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
            //this.UpdateArivalState(entity);

            //if (entity.Follow.IsFollowing)
            //    this.Follow(entity);

            if (entity.Battle.IsFighting)
                this.Fight(entity);
            else
                this.UpdateMoves(entity);
        }

        /// <inheritdoc />
        public void OnArrived(IMonsterEntity entity)
        {
            // TODO
        }

        /// <summary>
        /// Update monster moves.
        /// </summary>
        /// <param name="monster"></param>
        private void UpdateMoves(IMonsterEntity monster)
        {
            if (monster.Timers.NextMoveTime <= Time.TimeInSeconds() &&
                monster.MovableComponent.HasArrived &&
                monster.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
            {
                this.MoveToPosition(monster, monster.Region.GetRandomPosition());
            }
            else if (monster.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
            {
                if (monster.MovableComponent.ReturningToOriginalPosition)
                {
                    monster.Health.Hp = monster.Data.AddHp;
                    WorldPacketFactory.SendUpdateAttributes(monster, DefineAttributes.HP, monster.Health.Hp);
                    monster.MovableComponent.ReturningToOriginalPosition = false;
                }
                else
                {
                    if (monster.MovableComponent.SpeedFactor >= 2f)
                    {
                        monster.MovableComponent.SpeedFactor = 1f;
                        WorldPacketFactory.SendSpeedFactor(monster, monster.MovableComponent.SpeedFactor);
                    }
                }
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
            monster.Object.Angle = Vector3.AngleBetween(monster.Object.Position, destPosition);
            monster.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;
            monster.MovableComponent.DestinationPosition = destPosition.Clone();

            WorldPacketFactory.SendDestinationPosition(monster);
            WorldPacketFactory.SendDestinationAngle(monster, false);
        }

        /// <summary>
        /// Process the monster's fight.
        /// </summary>
        /// <param name="monster"></param>
        private void Fight(IMonsterEntity monster)
        {
            if (!monster.Battle.Target.Object.Spawned)
                return;
            
            if (monster.Follow.IsFollowing)
            {
                monster.MovableComponent.DestinationPosition = monster.Battle.Target.Object.Position.Clone();

                if (monster.MovableComponent.SpeedFactor != 2f)
                {
                    monster.MovableComponent.SpeedFactor = 2f;
                    WorldPacketFactory.SendSpeedFactor(monster, monster.MovableComponent.SpeedFactor);
                }

                if (!monster.Object.Position.IsInCircle(monster.MovableComponent.DestinationPosition, 1f))
                {
                    Console.WriteLine("Fight(): Not in range, sending follow");
                    monster.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;
                    WorldPacketFactory.SendFollowTarget(monster, monster.Follow.Target, 1f);
                }
                else
                {
                    if (monster.Timers.NextAttackTime <= Time.TimeInMilliseconds())
                    {
                        monster.Timers.NextAttackTime = (long)(Time.TimeInMilliseconds() + monster.Data.ReAttackDelay);

                        var meleeAttack = new MeleeAttackEventArgs(ObjectMessageType.OBJMSG_ATK1, monster.Battle.Target, monster.Data.AttackSpeed);
                        monster.NotifySystem<BattleSystem>(meleeAttack);
                    }
                }
            }
            else
            {
                Console.WriteLine("Reset()");
                monster.Battle.Reset();
                monster.Follow.Reset();
            }
        }
    }
}

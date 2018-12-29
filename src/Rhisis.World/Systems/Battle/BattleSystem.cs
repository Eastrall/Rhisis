using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;

namespace Rhisis.World.Systems.Battle
{
    [System(SystemType.Notifiable)]
    public class BattleSystem : ISystem
    {
        private static readonly ILogger<BattleSystem> Logger = DependencyContainer.Instance.Resolve<ILogger<BattleSystem>>();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player | WorldEntityType.Monster;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            if (!args.CheckArguments())
            {
                Logger.LogError("Cannot execute battle action: {0} due to invalid arguments.", args.GetType());
                return;
            }

            if (!(entity is ILivingEntity livingEntity))
            {
                Logger.LogError($"The non living entity {entity.Object.Name} tried to execute a battle action.");
                return;
            }

            switch (args)
            {
                case MeleeAttackEventArgs meleeAttackEventArgs:
                    this.ProcessMeleeAttack(livingEntity, meleeAttackEventArgs);
                    break;
            }
        }

        /// <summary>
        /// Process the melee attack algorithm.
        /// </summary>
        /// <param name="attacker">Attacker</param>
        /// <param name="e">Melee attack event arguments</param>
        private void ProcessMeleeAttack(ILivingEntity attacker, MeleeAttackEventArgs e)
        {
            ILivingEntity defender = e.Target;

            if (defender.Health.IsDead)
            {
                Logger.LogError($"{attacker.Object.Name} cannot attack {defender.Object.Name} because target is already dead.");
                this.ClearBattleTargets(defender);
                this.ClearBattleTargets(attacker);
                return;
            }

            attacker.Battle.Target = defender;
            defender.Battle.Target = attacker;

            AttackResult meleeAttackResult = new MeleeAttackArbiter(attacker, defender).OnDamage();

            Logger.LogDebug($"{attacker.Object.Name} inflicted {meleeAttackResult.Damages} to {defender.Object.Name}");

            if (meleeAttackResult.Flags.HasFlag(AttackFlags.AF_FLYING))
                BattleHelper.KnockbackEntity(defender);

            WorldPacketFactory.SendAddDamage(defender, attacker, meleeAttackResult.Flags, meleeAttackResult.Damages);
            WorldPacketFactory.SendMeleeAttack(attacker, e.AttackType, defender.Id, e.UnknownParameter, meleeAttackResult.Flags);

            defender.Health.Hp -= meleeAttackResult.Damages;

            if (defender.Health.IsDead)
            {
                Logger.LogDebug($"{attacker.Object.Name} killed {defender.Object.Name}.");
                defender.Health.Hp = 0;
                this.ClearBattleTargets(defender);
                this.ClearBattleTargets(attacker);
                WorldPacketFactory.SendDie(attacker as IPlayerEntity, defender, attacker, e.AttackType);

                if (defender is IMonsterEntity deadMonster)
                {
                    deadMonster.Timers.DespawnTime = Time.TimeInSeconds() + 5;
                    deadMonster.NotifySystem<DropSystem>(null); // Items
                    deadMonster.NotifySystem<DropSystem>(null); // Gold

                    // TODO: give exp
                }
            }
        }

        private void ClearBattleTargets(ILivingEntity entity)
        {
            entity.Follow.Target = null;
            entity.Battle.Target = null;
            entity.Battle.Targets.Clear();
        }
    }
}

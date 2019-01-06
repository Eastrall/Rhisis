using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;
using Rhisis.World.Systems.Drop.EventArgs;
using System.Linq;

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
                    var worldServerConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
                    var itemsData = DependencyContainer.Instance.Resolve<ItemLoader>();
                    var expTable = DependencyContainer.Instance.Resolve<ExpTableLoader>();

                    deadMonster.Timers.DespawnTime = Time.TimeInSeconds() + 5;

                    // Drop items
                    int itemCount = 0;
                    foreach (DropItemData dropItem in deadMonster.Data.DropItems)
                    {
                        if (itemCount >= deadMonster.Data.MaxDropItem)
                            break;

                        long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                        if (dropItem.Probability * worldServerConfiguration.Rates.Drop >= dropChance)
                        {
                            var item = new Item(dropItem.ItemId, 1, -1, -1, -1, (byte)RandomHelper.Random(0, dropItem.ItemMaxRefine));

                            deadMonster.NotifySystem<DropSystem>(new DropItemEventArgs(item, attacker));
                            itemCount++;
                        }
                    }

                    // Drop item kinds
                    foreach (DropItemKindData dropItemKind in deadMonster.Data.DropItemsKind)
                    {
                        var itemsDataByItemKind = itemsData.GetItems(x => x.ItemKind3 == dropItemKind.ItemKind && x.Rare >= dropItemKind.UniqueMin && x.Rare <= dropItemKind.UniqueMax);

                        if (!itemsDataByItemKind.Any())
                            continue;

                        var itemData = itemsDataByItemKind.ElementAt(RandomHelper.Random(0, itemsDataByItemKind.Count() - 1));

                        int itemRefine = RandomHelper.Random(0, 10);

                        for (int i = itemRefine; i >= 0; i--)
                        {
                            long itemDropProbability = (long)(expTable.GetDropLuck(itemData.Level > 120 ? 119 : itemData.Level, itemRefine) * (deadMonster.Data.CorrectionValue / 100f));
                            long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                            if (dropChance < itemDropProbability * worldServerConfiguration.Rates.Drop)
                            {
                                var item = new Item(itemData.Id, 1, -1, -1, -1, (byte)itemRefine);

                                deadMonster.NotifySystem<DropSystem>(new DropItemEventArgs(item, attacker));
                                break;
                            }
                        }
                    }

                    // Drop gold
                    int goldDropped = RandomHelper.Random(deadMonster.Data.DropGoldMin, deadMonster.Data.DropGoldMax);
                    deadMonster.NotifySystem<DropSystem>(new DropGoldEventArgs(goldDropped, attacker));

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

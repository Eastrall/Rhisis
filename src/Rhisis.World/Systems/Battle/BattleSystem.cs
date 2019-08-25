using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Drop;
using Rhisis.World.Systems.Leveling;
using System.Linq;

namespace Rhisis.World.Systems.Battle
{
    [Injectable]
    public class BattleSystem : IBattleSystem
    {
        private readonly ILogger<BattleSystem> _logger;
        private readonly IGameResources _gameResources;
        private readonly IDropSystem _dropSystem;
        private readonly IExperienceSystem _experienceSystem;
        private readonly WorldConfiguration _worldConfiguration;

        /// <summary>
        /// Creates a new <see cref="BattleSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="worldConfiguration">World server configuration.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="dropSystem">Drop system.</param>
        public BattleSystem(ILogger<BattleSystem> logger, IOptions<WorldConfiguration> worldConfiguration, IGameResources gameResources, IDropSystem dropSystem, IExperienceSystem experienceSystem)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration.Value;
            this._gameResources = gameResources;
            this._dropSystem = dropSystem;
            this._experienceSystem = experienceSystem;
        }
        
        /// <inheritdoc />
        public void MeleeAttack(ILivingEntity attacker, ILivingEntity defender, ObjectMessageType attackType, float attackSpeed)
        {
            if (defender.Health.IsDead)
            {
                this._logger.LogError($"{attacker.Object.Name} cannot attack {defender.Object.Name} because target is already dead.");
                this.ClearBattleTargets(defender);
                this.ClearBattleTargets(attacker);
                return;
            }

            attacker.Battle.Target = defender;
            defender.Battle.Target = attacker;

            AttackResult meleeAttackResult = new MeleeAttackArbiter(attacker, defender).OnDamage();

            this._logger.LogDebug($"{attacker.Object.Name} inflicted {meleeAttackResult.Damages} to {defender.Object.Name}");

            if (meleeAttackResult.Flags.HasFlag(AttackFlags.AF_FLYING))
                BattleHelper.KnockbackEntity(defender);

            WorldPacketFactory.SendAddDamage(defender, attacker, meleeAttackResult.Flags, meleeAttackResult.Damages);
            WorldPacketFactory.SendMeleeAttack(attacker, attackType, defender.Id, unknwonParam: 0, meleeAttackResult.Flags);

            defender.Health.Hp -= meleeAttackResult.Damages;
            WorldPacketFactory.SendUpdateAttributes(defender, DefineAttributes.HP, defender.Health.Hp);

            if (defender.Health.IsDead)
            {
                this._logger.LogDebug($"{attacker.Object.Name} killed {defender.Object.Name}.");
                defender.Health.Hp = 0;
                this.ClearBattleTargets(defender);
                this.ClearBattleTargets(attacker);
                WorldPacketFactory.SendUpdateAttributes(defender, DefineAttributes.HP, defender.Health.Hp);

                if (defender is IMonsterEntity deadMonster && attacker is IPlayerEntity player)
                {
                    WorldPacketFactory.SendDie(player, defender, attacker, attackType);

                    deadMonster.Timers.DespawnTime = Time.TimeInSeconds() + 5; // Configure this timer on world configuration

                    // Drop items
                    int itemCount = 0;
                    foreach (DropItemData dropItem in deadMonster.Data.DropItems)
                    {
                        if (itemCount >= deadMonster.Data.MaxDropItem)
                            break;

                        long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                        if (dropItem.Probability * this._worldConfiguration.Rates.Drop >= dropChance)
                        {
                            var item = new Item(dropItem.ItemId, 1, -1, -1, -1, (byte)RandomHelper.Random(0, dropItem.ItemMaxRefine));

                            this._dropSystem.DropItem(defender, item, attacker);
                            itemCount++;
                        }
                    }

                    // Drop item kinds
                    foreach (DropItemKindData dropItemKind in deadMonster.Data.DropItemsKind)
                    {
                        var itemsDataByItemKind = this._gameResources.Items.Values.Where(x => x.ItemKind3 == dropItemKind.ItemKind && x.Rare >= dropItemKind.UniqueMin && x.Rare <= dropItemKind.UniqueMax);

                        if (!itemsDataByItemKind.Any())
                            continue;

                        var itemData = itemsDataByItemKind.ElementAt(RandomHelper.Random(0, itemsDataByItemKind.Count() - 1));

                        int itemRefine = RandomHelper.Random(0, 10);

                        for (int i = itemRefine; i >= 0; i--)
                        {
                            long itemDropProbability = (long)(this._gameResources.ExpTables.GetDropLuck(itemData.Level > 120 ? 119 : itemData.Level, itemRefine) * (deadMonster.Data.CorrectionValue / 100f));
                            long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                            if (dropChance < itemDropProbability * this._worldConfiguration.Rates.Drop)
                            {
                                var item = new Item(itemData.Id, 1, -1, -1, -1, (byte)itemRefine);

                                this._dropSystem.DropItem(defender, item, attacker);
                                break;
                            }
                        }
                    }

                    // Drop gold
                    int goldDropped = RandomHelper.Random(deadMonster.Data.DropGoldMin, deadMonster.Data.DropGoldMax);
                    this._dropSystem.DropGold(deadMonster, goldDropped, attacker);

                    // Give experience
                    this._experienceSystem.GiveExeperience(player, deadMonster.Data.Experience * this._worldConfiguration.Rates.Experience);
                }
                else if (defender is IPlayerEntity deadPlayer)
                {
                    WorldPacketFactory.SendDie(deadPlayer, defender, attacker, attackType);
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

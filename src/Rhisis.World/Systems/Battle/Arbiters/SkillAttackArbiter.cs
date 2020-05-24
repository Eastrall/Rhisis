using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Systems.Inventory;
using System;

namespace Rhisis.World.Systems.Battle.Arbiters
{
    public class SkillAttackArbiter : AttackArbiterBase
    {
        /// <summary>
        /// Gets the attacker skill.
        /// </summary>
        protected SkillInfo Skill { get; }

        /// <summary>
        /// Creates a new <see cref="SkillAttackArbiter"/> instance.
        /// </summary>
        /// <param name="attacker">Attacker living entity.</param>
        /// <param name="defender">Defender living entity.</param>
        /// <param name="skill">Skill.</param>
        protected SkillAttackArbiter(ILivingEntity attacker, ILivingEntity defender, SkillInfo skill)
            : base(attacker, defender)
        {
            Skill = skill;
        }

        /// <inheritdoc />
        public override AttackResult CalculateDamages() => null;

        /// <summary>
        /// Gets the attacker skill power.
        /// </summary>
        /// <returns>Skill power.</returns>
        protected int GetAttackerSkillPower()
        {
            var referStatistic1 = Attacker.Attributes[Skill.Data.ReferStat1];
            var referStatistic2 = Attacker.Attributes[Skill.Data.ReferStat2];

            if (Skill.Data.ReferTarget1 == SkillReferTargetType.Attack && referStatistic1 != 0)
            {
                referStatistic1 = (int)(Skill.Data.ReferValue1 / 10f * referStatistic1 + Skill.Level * (referStatistic1 / 50f));
            }

            if (Skill.Data.ReferTarget2 == SkillReferTargetType.Attack && referStatistic2 != 0)
            {
                referStatistic2 = (int)(Skill.Data.ReferValue2 / 10f * referStatistic2 + Skill.Level * (referStatistic2 / 50f));
            }

            var referStatistic = referStatistic1 + referStatistic2;
            int attackMin;
            int attackMax;

            if (Attacker.Type == WorldEntityType.Player && Defender.Type == WorldEntityType.Player)
            {
                attackMin = Skill.LevelData.AbilityMinPVP;
                attackMax = Skill.LevelData.AbilityMaxPVP;
            }
            else
            {
                attackMin = Skill.LevelData.AbilityMin;
                attackMax = Skill.LevelData.AbilityMax;
            }

            InventoryItem weaponItem = Attacker.Hand;

            if (Attacker is IPlayerEntity player)
            {
                weaponItem = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon);

                if (weaponItem.Id == -1)
                {
                    weaponItem = InventorySystem.Hand;
                }
            }

            AttackResult weaponAttackPower = BattleHelper.GetWeaponAttackPower(Attacker, weaponItem);
            var weaponExtraDamages = BattleHelper.GetWeaponExtraDamages(Attacker, weaponItem);

            attackMin += weaponItem.Data.AttackSkillMin;
            attackMax += weaponItem.Data.AttackSkillMax;

            float powerMin = (weaponAttackPower.AttackMin + attackMin * 5 + referStatistic - 20) * (16 + Skill.Level) / 13;
            float powerMax = (weaponAttackPower.AttackMax + attackMax * 5 + referStatistic - 20) * (16 + Skill.Level) / 13;

            // TODO: check CHR_DMG
            powerMin += weaponExtraDamages;
            powerMax += weaponExtraDamages;

            var attackMinMax = Math.Max(powerMax - powerMin + 1, 1);

            return (int)(powerMin + RandomHelper.FloatRandom(1f, attackMinMax));
        }

        protected override float GetDefenseMultiplier(float defenseFactor = 1)
        {
            if (Skill.SkillId == DefineSkill.SI_BLD_DOUBLE_ARMORPENETRATE)
            {
                defenseFactor *= 0.5f;
            }

            return base.GetDefenseMultiplier(defenseFactor);
        }
    }
}

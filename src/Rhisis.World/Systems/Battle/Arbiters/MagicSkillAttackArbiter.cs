using Rhisis.Core.Data;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System;

namespace Rhisis.World.Systems.Battle.Arbiters
{
    public class MagicSkillAttackArbiter : SkillAttackArbiter, IAttackArbiter
    {
        /// <summary>
        /// Creates a new <see cref="MagicSkillAttackArbiter"/> instance.
        /// </summary>
        /// <param name="attacker">Attacker entity.</param>
        /// <param name="defender">Defender entity.</param>
        /// <param name="skill">Magic skill.</param>
        public MagicSkillAttackArbiter(ILivingEntity attacker, ILivingEntity defender, SkillInfo skill)
            : base(attacker, defender, skill)
        {
        }

        /// <inheritdoc />
        public override AttackResult CalculateDamages()
        {
            var damages = GetAttackerSkillPower();
            DefineAttributes? skillMastryAttribute = Skill.Data.SpellType switch
            {
                SpellType.Fire => DefineAttributes.MASTRY_FIRE,
                SpellType.Water => DefineAttributes.MASTRY_WATER,
                SpellType.Electricity => DefineAttributes.MASTRY_ELECTRICITY,
                SpellType.Wind => DefineAttributes.MASTRY_WIND,
                SpellType.Earth => DefineAttributes.MASTRY_EARTH,
                _ => default
            };

            if (skillMastryAttribute.HasValue)
            {
                var ratio = Math.Max(0, Attacker.Attributes[skillMastryAttribute.Value] / 100f);

                damages = (int)(damages + damages * ratio);
            }

            damages *= (int)GetAttackMultiplier();
            damages += Attacker.Attributes[DefineAttributes.ATKPOWER];

            var attackResult = new AttackResult
            {
                Flags = AttackFlags.AF_MAGICSKILL,
                Damages = damages
            };

            PostCalculation(attackResult);

            return attackResult;
        }

        /// <summary>
        /// Process the post calculation formulas for a magic skill.
        /// </summary>
        /// <param name="attackResult">Current attack result.</param>
        private void PostCalculation(AttackResult attackResult)
        {
            var damages = attackResult.Damages;
            var defenderDefense = GetDefenderDefense(attackResult);

            damages -= damages * (int)(Defender.Attributes[DefineAttributes.RESIST_MAGIC_RATE] / 100f);

            var damageDefenseDelta = (damages - defenderDefense) * (1.0f - GetMagicSkillResitanceRate(Defender, Skill));

            attackResult.Damages = (int)(damageDefenseDelta * GetMagicSkillFactor(Attacker, Skill));
        }

        /// <summary>
        /// Gets the magic skill resistance for a given living entity.
        /// </summary>
        /// <param name="entity">Living entity.</param>
        /// <param name="skill">Skill.</param>
        /// <returns>Magic skill resistance.</returns>
        private float GetMagicSkillResistance(ILivingEntity entity, SkillInfo skill)
        {
            var skillResistance = skill.Data.Element switch
            {
                ElementType.Fire => entity.Data.FireResitance + entity.Attributes[DefineAttributes.RESIST_FIRE],
                ElementType.Water => entity.Data.WaterResistance + entity.Attributes[DefineAttributes.RESIST_WATER],
                ElementType.Electricity => entity.Data.ElectricityResistance + entity.Attributes[DefineAttributes.RESIST_ELECTRICITY],
                ElementType.Wind => entity.Data.WindResistance + entity.Attributes[DefineAttributes.RESIST_WIND],
                ElementType.Earth => entity.Data.EarthResistance + entity.Attributes[DefineAttributes.RESIST_EARTH],
                _ => default
            };

            return skillResistance;
        }

        /// <summary>
        /// Gets the magic skill resistance rate for a given living entity.
        /// </summary>
        /// <param name="entity">Living entity.</param>
        /// <param name="skill">Skill.</param>
        /// <returns>Magic skill resistance rate.</returns>
        private float GetMagicSkillResitanceRate(ILivingEntity entity, SkillInfo skill) => GetMagicSkillResistance(entity, skill) / 100f;

        /// <summary>
        /// Gets the magic skill factor based on the equiped weapon element type and skill element type.
        /// </summary>
        /// <param name="entity">Living entity.</param>
        /// <param name="skill">Skill.</param>
        /// <returns>Magic skill factor.</returns>
        private float GetMagicSkillFactor(ILivingEntity entity, SkillInfo skill)
        {
            const float DefaultMagicSkillFactor = 1.0f;

            if (entity is IPlayerEntity player)
            {
                InventoryItem weapon = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon) ?? player.Hand;
                ElementType weaponElementType = weapon.Id != -1 ? weapon.Data.Element : ElementType.None;
                ElementType skillElementType = skill.Data.Element.GetValueOrDefault(ElementType.None);

                if (skillElementType == weaponElementType)
                {
                    return 1.1f;
                }
                else if (
                    skillElementType == ElementType.Fire && weaponElementType == ElementType.Water ||
                    skillElementType == ElementType.Water && weaponElementType == ElementType.Electricity ||
                    skillElementType == ElementType.Electricity && weaponElementType == ElementType.Earth ||
                    skillElementType == ElementType.Wind && weaponElementType == ElementType.Fire
                    )
                {
                    return 0.9f;
                }
            }

            return DefaultMagicSkillFactor;
        }
    }
}

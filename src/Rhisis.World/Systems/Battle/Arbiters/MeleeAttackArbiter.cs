using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Systems.Battle.Arbiters
{
    /// <summary>
    /// Provides a mechanism to calculate a melee attack result based on the attacker and defender statistics.
    /// </summary>
    public class MeleeAttackArbiter : AttackArbiterBase, IAttackArbiter
    {
        public const int MinimalHitRate = 20;
        public const int MaximalHitRate = 96;

        /// <summary>
        /// Creates a new <see cref="MeleeAttackArbiter"/> instance.
        /// </summary>
        /// <param name="attacker">Attacker entity</param>
        /// <param name="defender">Defender entity</param>
        public MeleeAttackArbiter(ILivingEntity attacker, ILivingEntity defender)
            : base(attacker, defender)
        {
        }

        /// <inheritdoc />
        public override AttackResult CalculateDamages()
        {
            var attackResult = new AttackResult
            {
                Flags = GetAttackFlags()
            };

            if (attackResult.Flags.HasFlag(AttackFlags.AF_MISS))
                return attackResult;

            if (Attacker is IPlayerEntity player)
            {
                InventoryItem rightWeapon = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon) ?? player.Hand;

                // TODO: GetDamagePropertyFactor()
                var weaponAttack = BattleHelper.GetWeaponAttackDamages(rightWeapon.Data.WeaponType, player);
                attackResult.AttackMin = rightWeapon.Data.AbilityMin * 2 + weaponAttack;
                attackResult.AttackMax = rightWeapon.Data.AbilityMax * 2 + weaponAttack;
            }
            else if (Attacker is IMonsterEntity monster)
            {
                if (attackResult.Flags.HasFlag(AttackFlags.AF_MISS))
                {
                    return attackResult;
                }

                attackResult.AttackMin = monster.Data.AttackMin;
                attackResult.AttackMax = monster.Data.AttackMax;
            }

            if (IsCriticalAttack(Attacker, attackResult.Flags))
            {
                attackResult.Flags |= AttackFlags.AF_CRITICAL;
                CalculateCriticalDamages(attackResult);

                if (IsKnockback(attackResult.Flags))
                {
                    attackResult.Flags |= AttackFlags.AF_FLYING;
                }
            }

            attackResult.Damages = RandomHelper.Random(attackResult.AttackMin, attackResult.AttackMax);
            attackResult.Damages -= GetDefenderDefense(attackResult);

            if (attackResult.Damages > 0)
            {
                var blockFactor = GetDefenderBlockFactor();
                if (blockFactor < 1f)
                {
                    attackResult.Flags |= AttackFlags.AF_BLOCKING;
                    attackResult.Damages = (int)(attackResult.Damages * blockFactor);
                }
            }
            else
            {
                attackResult.Damages = 0;
                attackResult.Flags &= ~AttackFlags.AF_CRITICAL;
                attackResult.Flags &= ~AttackFlags.AF_FLYING;
            }

            return attackResult;
        }

        /// <summary>
        /// Gets the <see cref="AttackFlags"/> of this melee attack.
        /// </summary>
        /// <returns></returns>
        private AttackFlags GetAttackFlags()
        {
            // TODO: if attacker mode == ONEKILL_MODE, return AF_GENERIC

            int hitRate;
            var hitRating = GetHitRating(Attacker);
            var escapeRating = GetEspaceRating(Defender);

            if (Attacker.Type == WorldEntityType.Monster && Defender.Type == WorldEntityType.Player)
            {
                // Monster VS Player
                hitRate = (int)(hitRating * 1.5f / (hitRating + escapeRating) * 2.0f *
                          (Attacker.Object.Level * 0.5f / (Attacker.Object.Level + Defender.Object.Level * 0.3f)) * 100.0f);
            }
            else
            {
                // Player VS Player or Player VS Monster
                hitRate = (int)(hitRating * 1.6f / (hitRating + escapeRating) * 1.5f *
                          (Attacker.Object.Level * 1.2f / (Attacker.Object.Level + Defender.Object.Level)) * 100.0f);
            }

            hitRate = MathHelper.Clamp(hitRate, MinimalHitRate, MaximalHitRate);

            return RandomHelper.Random(0, 100) < hitRate ? AttackFlags.AF_GENERIC : AttackFlags.AF_MISS;
        }

        /// <summary>
        /// Gets the hit rating of an entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns></returns>
        private int GetHitRating(ILivingEntity entity)
        {
            if (entity is IPlayerEntity player)
                return player.Attributes[DefineAttributes.DEX]; // TODO: add dex bonus
            else if (entity is IMonsterEntity monster)
                return monster.Data.HitRating;

            return 0;
        }

        /// <summary>
        /// Check if the attacker's melee attack is a critical hit.
        /// </summary>
        /// <param name="attacker">Attacker</param>
        /// <param name="currentAttackFlags">Attack flags</param>
        /// <returns></returns>
        public bool IsCriticalAttack(ILivingEntity attacker, AttackFlags currentAttackFlags)
        {
            if (currentAttackFlags.HasFlag(AttackFlags.AF_MELEESKILL) || currentAttackFlags.HasFlag(AttackFlags.AF_MAGICSKILL))
                return false;

            var criticalJobFactor = attacker is IPlayerEntity player ? player.PlayerData.JobData.Critical : 1f;
            var criticalProbability = (int)(attacker.Attributes[DefineAttributes.DEX] / 10 * criticalJobFactor);
            // TODO: add DST_CHR_CHANCECRITICAL to criticalProbability

            if (criticalProbability < 0)
                criticalProbability = 0;

            // TODO: check if player is in party and if it has the MVRF_CRITICAL flag

            return RandomHelper.Random(0, 100) < criticalProbability;
        }

        /// <summary>
        /// Calculate critical damages.
        /// </summary>
        /// <param name="attackResult">Attack result</param>
        public void CalculateCriticalDamages(AttackResult attackResult)
        {
            var criticalMin = 1.1f;
            var criticalMax = 1.4f;

            if (Attacker.Object.Level > Defender.Object.Level)
            {
                if (Defender.Type == WorldEntityType.Monster)
                {
                    criticalMin = 1.2f;
                    criticalMax = 2.0f;
                }
                else
                {
                    criticalMin = 1.4f;
                    criticalMax = 1.8f;
                }
            }

            float criticalBonus = 1; // TODO: 1 + (DST_CRITICAL_BONUS / 100)

            if (criticalBonus < 0.1f)
                criticalBonus = 0.1f;

            attackResult.AttackMin = (int)(attackResult.AttackMin * criticalMin * criticalBonus);
            attackResult.AttackMax = (int)(attackResult.AttackMax * criticalMax * criticalBonus);
        }

        /// <summary>
        /// Check if the current attack is a chance to knockback the defender.
        /// </summary>
        /// <param name="attackerAttackFlags">Attacker attack flags</param>
        /// <returns></returns>
        public bool IsKnockback(AttackFlags attackerAttackFlags)
        {
            var knockbackChance = RandomHelper.Random(0, 100) < 15;

            if (Defender.Type == WorldEntityType.Player)
                return false;

            if (Attacker is IPlayerEntity player)
            {
                InventoryItem weapon = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon) ?? player.Hand;

                if (weapon.Data.WeaponType == WeaponType.MELEE_YOYO || attackerAttackFlags.HasFlag(AttackFlags.AF_FORCE))
                {
                    return false;
                }
            }

            var canFly = false;

            // TODO: if is flying, return false
            if ((Defender.Object.MovingFlags & ObjectState.OBJSTA_DMG_FLY_ALL) == 0 && Defender is IMonsterEntity monster)
            {
                canFly = monster.Data.Class != MoverClassType.RANK_SUPER &&
                    monster.Data.Class != MoverClassType.RANK_MATERIAL &&
                    monster.Data.Class != MoverClassType.RANK_MIDBOSS;
            }

            return canFly && knockbackChance;
        }
    }
}

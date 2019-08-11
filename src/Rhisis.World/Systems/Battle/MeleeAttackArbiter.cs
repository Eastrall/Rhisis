using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Systems.Inventory;

namespace Rhisis.World.Systems.Battle
{
    /// <summary>
    /// Provides a mechanism to calculate a melee attack result based on the attacker and defender statistics.
    /// </summary>
    public class MeleeAttackArbiter
    {
        public const int MinimalHitRate = 20;
        public const int MaximalHitRate = 96;
        private readonly ILivingEntity _attacker;
        private readonly ILivingEntity _defender;

        /// <summary>
        /// Creates a new <see cref="MeleeAttackArbiter"/> instance.
        /// </summary>
        /// <param name="attacker">Attacker entity</param>
        /// <param name="defender">Defender entity</param>
        public MeleeAttackArbiter(ILivingEntity attacker, ILivingEntity defender)
        {
            this._attacker = attacker;
            this._defender = defender;
        }

        /// <summary>
        /// Gets the melee damages inflicted by an attacker to a defender.
        /// </summary>
        /// <returns><see cref="AttackResult"/></returns>
        public AttackResult OnDamage()
        {
            var attackResult = new AttackResult
            {
                Flags = this.GetAttackFlags()
            };
            
            if (attackResult.Flags.HasFlag(AttackFlags.AF_MISS))
                return attackResult;
            
            if (this._attacker is IPlayerEntity player)
            {
                Item rightWeapon = player.Inventory.GetItem(x => x.Slot == InventorySystem.RightWeaponSlot) ?? InventorySystem.Hand;

                // TODO: GetDamagePropertyFactor()
                int weaponAttack = BattleHelper.GetWeaponAttackDamages(rightWeapon.Data.WeaponType, player);
                attackResult.AttackMin = rightWeapon.Data.AbilityMin * 2 + weaponAttack;
                attackResult.AttackMax = rightWeapon.Data.AbilityMax * 2 + weaponAttack;
            }
            else if (this._attacker is IMonsterEntity monster)
            {
                attackResult.AttackMin = monster.Data.AttackMin;
                attackResult.AttackMax = monster.Data.AttackMax;
            }

            if (this.IsCriticalAttack(this._attacker, attackResult.Flags))
            {
                attackResult.Flags |= AttackFlags.AF_CRITICAL;
                this.CalculateCriticalDamages(attackResult);

                if (this.IsKnockback(attackResult.Flags))
                    attackResult.Flags |= AttackFlags.AF_FLYING;
            }

            attackResult.Damages = RandomHelper.Random(attackResult.AttackMin, attackResult.AttackMax);
            attackResult.Damages -= this.GetDefenderDefense(attackResult);

            if (attackResult.Damages > 0)
            {
                float blockFactor = this.GetDefenderBlockFactor();
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
            int hitRating = this.GetHitRating(this._attacker);
            int escapeRating = this.GetEspaceRating(this._defender);

            if (this._attacker.Type == WorldEntityType.Monster && this._defender.Type == WorldEntityType.Player)
            {
                // Monster VS Player
                hitRate = (int)(((hitRating * 1.5f) / (hitRating + escapeRating)) * 2.0f *
                          (this._attacker.Object.Level * 0.5f / (this._attacker.Object.Level + this._defender.Object.Level * 0.3f)) * 100.0f);
            }
            else
            {
                // Player VS Player or Player VS Monster
                hitRate = (int)(((hitRating * 1.6f) / (hitRating + escapeRating)) * 1.5f *
                          (this._attacker.Object.Level * 1.2f / (this._attacker.Object.Level + this._defender.Object.Level)) * 100.0f);
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
        /// Gets the escape rating of an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int GetEspaceRating(ILivingEntity entity)
        {
            if (entity is IPlayerEntity player)
                return (int)(player.Attributes[DefineAttributes.DEX] * 0.5f); // TODO: add dex bonus and DST_PARRY
            else if (entity is IMonsterEntity monster)
                return monster.Data.EscapeRating;

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

            float criticalJobFactor = attacker is IPlayerEntity player ? player.PlayerData.JobData.Critical : 1f;
            int criticalProbability = (int)((attacker.Attributes[DefineAttributes.DEX] / 10) * criticalJobFactor);
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
            float criticalMin = 1.1f;
            float criticalMax = 1.4f;

            if (this._attacker.Object.Level > this._defender.Object.Level)
            {
                if (this._defender.Type == WorldEntityType.Monster)
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
            bool knockbackChance = RandomHelper.Random(0, 100) < 15;

            if (this._defender.Type == WorldEntityType.Player)
                return false;

            if (this._attacker is IPlayerEntity player)
            {
                var weapon = player.Inventory[InventorySystem.RightWeaponSlot];

                if (weapon == null)
                    weapon = InventorySystem.Hand;
                if (weapon.Data.WeaponType == WeaponType.MELEE_YOYO || attackerAttackFlags.HasFlag(AttackFlags.AF_FORCE))
                    return false;
            }

            bool canFly = false;

            // TODO: if is flying, return false
            if ((this._defender.Object.MovingFlags & ObjectState.OBJSTA_DMG_FLY_ALL) == 0 && this._defender is IMonsterEntity monster)
            {
                canFly = monster.Data.Class != MoverClassType.RANK_SUPER && 
                    monster.Data.Class != MoverClassType.RANK_MATERIAL && 
                    monster.Data.Class != MoverClassType.RANK_MIDBOSS;
            }

            return canFly && knockbackChance;
        }

        /// <summary>
        /// Gets the defender defense.
        /// </summary>
        /// <param name="attackResult"></param>
        /// <returns></returns>
        public int GetDefenderDefense(AttackResult attackResult)
        {
            bool isGenericAttack = attackResult.Flags.HasFlag(AttackFlags.AF_GENERIC);

            if (this._attacker.Type == WorldEntityType.Player && this._defender.Type == WorldEntityType.Player)
                isGenericAttack = true;

            if (isGenericAttack)
            {
                float factor = 1f;

                if (this._defender is IPlayerEntity player)
                    factor = player.PlayerData.JobData.DefenseFactor;

                int stamina = this._defender.Attributes[DefineAttributes.STA];
                int level = this._defender.Object.Level;
                int defense = (int)(((((level * 2) + (stamina / 2)) / 2.8f) - 4) + ((stamina - 14) * factor));
                // TODO: add defense armor
                // TODO: add DST_ADJDEF

                if (defense < 0)
                    defense = 0;

                return defense;
            }

            if (this._defender is IMonsterEntity monster)
            {
                var monsterDefenseArmor = attackResult.Flags.HasFlag(AttackFlags.AF_MAGIC) ? monster.Data.MagicResitance : monster.Data.NaturalArmor;

                return (int)(monsterDefenseArmor / 7f + 1);
            }

            return 0;
        }

        /// <summary>
        /// Gets the defender blocking factor.
        /// </summary>
        /// <returns>Defender blocking factor</returns>
        public float GetDefenderBlockFactor()
        {
            if (this._defender.Type == WorldEntityType.Player)
            {
                int blockRandomProbability = RandomHelper.Random(0, 80);

                if (blockRandomProbability <= 5)
                    return 1f;
                if (blockRandomProbability >= 75f)
                    return 0.1f;

                int defenderLevel = this._defender.Object.Level;
                int defenderDexterity = this._defender.Attributes[DefineAttributes.DEX];

                float blockProbabilityA = defenderLevel / ((defenderLevel + this._attacker.Object.Level) * 15.0f);
                float blockProbabilityB = (defenderDexterity + this._attacker.Attributes[DefineAttributes.DEX] + 2) * ((defenderDexterity - this._attacker.Attributes[DefineAttributes.DEX]) / 800.0f);

                if (blockProbabilityB > 10.0f)
                    blockProbabilityB = 10.0f;
                float blockProbability = blockProbabilityA + blockProbabilityB;
                if (blockProbability < 0.0f)
                    blockProbability = 0.0f;

                // TODO: range attack probability

                var player = this._defender as IPlayerEntity;
                int blockRate = (int)((defenderDexterity / 8.0f) * player.PlayerData.JobData.Blocking + blockProbability);

                if (blockRate < 0)
                    blockRate = 0;
                
                if (blockRate > blockRandomProbability)
                    return 0f;
            }
            else if (this._defender.Type == WorldEntityType.Monster)
            {
                int blockRandomProbability = RandomHelper.Random(0, 100);

                if (blockRandomProbability <= 5)
                    return 1f;
                if (blockRandomProbability >= 95)
                    return 0f;

                int blockRate = (int)((this.GetEspaceRating(this._defender) - this._defender.Object.Level) * 0.5f);

                if (blockRate < 0)
                    blockRate = 0;

                if (blockRate > blockRandomProbability)
                    return 0.2f;
            }

            return 1f;  
        }
    }
}

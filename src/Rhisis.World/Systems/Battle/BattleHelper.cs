using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System;

namespace Rhisis.World.Systems.Battle
{
    /// <summary>
    /// This class provides methods to calculate everything related to the <see cref="BattleSystem"/>.
    /// </summary>
    public static class BattleHelper
    {
        /// <summary>
        /// Gets the weapon attack damages based on player's statistics.
        /// </summary>
        /// <param name="weaponType">Weapon type</param>
        /// <param name="player">Player</param>
        /// <returns></returns>
        public static int GetWeaponAttackDamages(WeaponType weaponType, IPlayerEntity player)
        {
            float attribute = 0f;
            float levelFactor = 0f;
            float jobFactor = 1f;

            switch (weaponType)
            {
                case WeaponType.MELEE_SWD:
                    attribute = player.Attributes[DefineAttributes.STR] - 12;
                    levelFactor = player.Object.Level * 1.1f;
                    jobFactor = player.PlayerData.JobData.MeleeSword;
                    break;
                case WeaponType.MELEE_AXE:
                    attribute = player.Attributes[DefineAttributes.STR] - 12;
                    levelFactor = player.Object.Level * 1.2f;
                    jobFactor = player.PlayerData.JobData.MeleeAxe;
                    break;
                case WeaponType.MELEE_STAFF:
                    attribute = player.Attributes[DefineAttributes.STR] - 10;
                    levelFactor = player.Object.Level * 1.1f;
                    jobFactor = player.PlayerData.JobData.MeleeStaff;
                    break;
                case WeaponType.MELEE_STICK:
                    attribute = player.Attributes[DefineAttributes.STR] - 10;
                    levelFactor = player.Object.Level * 1.3f;
                    jobFactor = player.PlayerData.JobData.MeleeStick;
                    break;
                case WeaponType.MELEE_KNUCKLE:
                    attribute = player.Attributes[DefineAttributes.STR] - 10;
                    levelFactor = player.Object.Level * 1.2f;
                    jobFactor = player.PlayerData.JobData.MeleeKnuckle;
                    break;
                case WeaponType.MAGIC_WAND:
                    attribute = player.Attributes[DefineAttributes.INT] - 10;
                    levelFactor = player.Object.Level * 1.2f;
                    jobFactor = player.PlayerData.JobData.MagicWand;
                    break;
                case WeaponType.MELEE_YOYO:
                    attribute = player.Attributes[DefineAttributes.STR] - 10;
                    levelFactor = player.Object.Level * 1.1f;
                    jobFactor = player.PlayerData.JobData.MeleeYoyo;
                    break;
                case WeaponType.RANGE_BOW:
                    attribute = (player.Attributes[DefineAttributes.DEX] - 14) * 4f;
                    levelFactor = player.Object.Level * 1.3f;
                    jobFactor = (player.Attributes[DefineAttributes.STR] * 0.2f) * 0.7f;
                    break;
            }

            return (int)(attribute * jobFactor + levelFactor);
        }

        // TODO: move this to utility
        public static int MulDiv(int number, int numerator, int denominator)
        {
            return (int)(((long)number * numerator) / denominator);
        }

        /// <summary>
        /// Knocks back an entity by calculating his destination position.
        /// </summary>
        /// <param name="entity">Entity to knockback</param>
        public static void KnockbackEntity(ILivingEntity entity)
        {
            var delta = new Vector3();
            float angle = MathHelper.ToRadian(entity.Object.Angle);
            float angleY = MathHelper.ToRadian(145f);

            delta.Y = (float)(-Math.Cos(angleY) * 0.18f);
            float dist = (float)(Math.Sin(angleY) * 0.18f);
            delta.X = (float)(Math.Sin(angle) * dist);
            delta.Z = (float)(-Math.Cos(angle) * dist);

            entity.Moves.DestinationPosition.X += delta.X;
            entity.Moves.DestinationPosition.Z += delta.Z;
        }

        /// <summary>
        /// Gets the weapon attack power.
        /// </summary>
        /// <param name="entity">Living entity using the weapon.</param>
        /// <param name="weapon">Weapon used by the living entity.</param>
        /// <returns><see cref="AttackResult"/> with AttackMin and AttackMax range.</returns>
        public static AttackResult GetWeaponAttackPower(ILivingEntity entity, InventoryItem weapon)
        {
            float multiplier = GetWeaponItemMultiplier(weapon);
            int power = weapon?.Refine > 0 ? (int)Math.Pow(weapon?.Refine ?? default, 1.5f) : default;

            return new AttackResult
            {
                AttackMin = (int)((entity.Attributes[DefineAttributes.ABILITY_MIN] + weapon?.Data.AbilityMin) * multiplier) + power,
                AttackMax = (int)((entity.Attributes[DefineAttributes.ABILITY_MAX] + weapon?.Data.AbilityMax) * multiplier) + power,
            };
        }

        /// <summary>
        /// Gets the weapon item multiplier.
        /// </summary>
        /// <param name="weapon">Current used weapon.</param>
        /// <returns></returns>
        public static float GetWeaponItemMultiplier(InventoryItem weapon)
        {
            // TODO: check if item has expired.
            float multiplier = 1.0f;
            int refine = weapon.Data.WeaponKind == WeaponKindType.Ultimate ? InventoryItem.RefineMax : weapon.Refine;

            if (refine > 0)
            {
                // TODO: get item exp up
                int itemExpUp = 0 + 100;
                multiplier *= (itemExpUp / 100);
            }

            return multiplier;
        }

        /// <summary>
        /// Gets the weapon extra damages based weapon type and entity attribute bonuses.
        /// </summary>
        /// <param name="entity">Current entity.</param>
        /// <param name="weapon">Current entity weapon.</param>
        /// <returns>Weapon extra damages</returns>
        public static int GetWeaponExtraDamages(ILivingEntity entity, InventoryItem weapon)
        {
            int extraDamages = weapon.Data.WeaponType switch
            {
                WeaponType.MELEE_SWD => entity.Attributes[DefineAttributes.SWD_DMG] + entity.Attributes[DefineAttributes.TWOHANDMASTER_DMG],
                WeaponType.MELEE_AXE => entity.Attributes[DefineAttributes.AXE_DMG] + entity.Attributes[DefineAttributes.TWOHANDMASTER_DMG],
                WeaponType.KNUCKLE => entity.Attributes[DefineAttributes.KNUCKLE_DMG] + entity.Attributes[DefineAttributes.KNUCKLEMASTER_DMG],
                WeaponType.MELEE_YOYO => entity.Attributes[DefineAttributes.YOY_DMG] + entity.Attributes[DefineAttributes.YOYOMASTER_DMG],
                WeaponType.RANGE_BOW => entity.Attributes[DefineAttributes.BOW_DMG] + entity.Attributes[DefineAttributes.BOWMASTER_DMG],
                _ => default
            };

            if (entity is IPlayerEntity player)
            {
                // TODO: check if player has dual weapons
                // TODO: if yes add "ONEHANDMASTER_DMG" to extra damages
            }

            return extraDamages;
        }
    }
}

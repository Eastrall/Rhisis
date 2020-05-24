using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System;

namespace Rhisis.World.Systems.Battle.Arbiters
{
    /// <summary>
    /// Provides a mechanism to calculate a magic attack based on the attacker and defender statistics.
    /// </summary>
    public class MagicAttackArbiter : AttackArbiterBase, IAttackArbiter
    {
        private readonly int _magicAttackPower;

        /// <summary>
        /// Creates a new <see cref="MagicAttackArbiter"/> instance.
        /// </summary>
        /// <param name="attacker">Attacker entity.</param>
        /// <param name="defender">Defender entity.</param>
        /// <param name="magicAttackPower">Magic attack power.</param>
        public MagicAttackArbiter(ILivingEntity attacker, ILivingEntity defender, int magicAttackPower)
            : base(attacker, defender)
        {
            _magicAttackPower = magicAttackPower;
        }

        /// <inheritdoc />
        public override AttackResult CalculateDamages()
        {
            var attackResult = new AttackResult
            {
                Flags = AttackFlags.AF_MAGIC
            };

            AttackResult weaponAttackResult = null;

            if (Attacker is IPlayerEntity player)
            {
                InventoryItem wandWeapon = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon) ?? player.Hand;

                weaponAttackResult = BattleHelper.GetWeaponAttackPower(Attacker, wandWeapon);
                var weaponAttackDamages = BattleHelper.GetWeaponAttackDamages(WeaponType.MAGIC_WAND, player);

                weaponAttackResult.AttackMin += weaponAttackDamages;
                weaponAttackResult.AttackMax += weaponAttackDamages;
            }

            if (weaponAttackResult != null)
            {
                attackResult.Damages = RandomHelper.Random(weaponAttackResult.AttackMin, weaponAttackResult.AttackMax);
                attackResult.Damages += Math.Max(0, Attacker.Attributes[DefineAttributes.CHR_DMG]);
                attackResult.Damages = (int)(attackResult.Damages * GetWandAttackMultiplier());
            }

            return attackResult;
        }

        /// <summary>
        /// Gets the wand attack multiplier.
        /// </summary>
        /// <returns></returns>
        private float GetWandAttackMultiplier()
        {
            return _magicAttackPower switch
            {
                0 => 0.6f,
                1 => 0.8f,
                2 => 1.05f,
                3 => 1.1f,
                4 => 1.3f,
                _ => 1.0f
            };
        }
    }
}

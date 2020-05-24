using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle.Arbiters;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Projectile;

namespace Rhisis.World.Systems.Battle
{
    [Injectable]
    public class BattleSystem : IBattleSystem
    {
        private readonly ILogger<BattleSystem> _logger;
        private readonly IProjectileSystem _projectileSystem;
        private readonly IInventorySystem _inventorySystem;
        private readonly IBattlePacketFactory _battlePacketFactory;
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="BattleSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="projectileSystem">Projectile system.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="battlePacketFactory">Battle packet factory.</param>
        /// <param name="moverPacketFactory">Mover packet factory.</param>
        public BattleSystem(ILogger<BattleSystem> logger, IProjectileSystem projectileSystem, IInventorySystem inventorySystem, IBattlePacketFactory battlePacketFactory, IMoverPacketFactory moverPacketFactory)
        {
            _logger = logger;
            _projectileSystem = projectileSystem;
            _inventorySystem = inventorySystem;
            _battlePacketFactory = battlePacketFactory;
            _moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public AttackResult DamageTarget(ILivingEntity attacker, ILivingEntity defender, IAttackArbiter attackArbiter, ObjectMessageType attackType)
        {
            if (!CanAttack(attacker, defender))
            {
                ClearBattleTargets(defender);
                ClearBattleTargets(attacker);
                return null;
            }

            if (defender is IPlayerEntity undyingPlayer)
            {
                if (undyingPlayer.PlayerData.Mode == ModeType.MATCHLESS_MODE)
                {
                    _logger.LogDebug($"{attacker.Object.Name} wasn't able to inflict any damages to {defender.Object.Name} because he is in undying mode");
                    return null;
                }
            }

            AttackResult attackResult;

            if (attacker is IPlayerEntity player && player.PlayerData.Mode.HasFlag(ModeType.ONEKILL_MODE))
            {
                attackResult = new AttackResult
                {
                    Damages = defender.Attributes[DefineAttributes.HP],
                    Flags = AttackFlags.AF_GENERIC
                };
            }
            else
            {
                attackResult = attackArbiter.CalculateDamages();
            }

            if (attackResult.Flags.HasFlag(AttackFlags.AF_MISS) || attackResult.Damages <= 0)
            {
                _battlePacketFactory.SendAddDamage(defender, attacker, attackResult.Flags, attackResult.Damages);

                return attackResult;
            }

            attacker.Battle.Target = defender;
            defender.Battle.Target = attacker;

            if (attackResult.Flags.HasFlag(AttackFlags.AF_FLYING))
            {
                BattleHelper.KnockbackEntity(defender);
            }

            _battlePacketFactory.SendAddDamage(defender, attacker, attackResult.Flags, attackResult.Damages);

            defender.Attributes[DefineAttributes.HP] -= attackResult.Damages;
            _moverPacketFactory.SendUpdateAttributes(defender, DefineAttributes.HP, defender.Attributes[DefineAttributes.HP]);

            CheckIfDefenderIsDead(attacker, defender, attackType);

            return attackResult;
        }

        /// <inheritdoc />
        public void MeleeAttack(ILivingEntity attacker, ILivingEntity defender, ObjectMessageType attackType, float attackSpeed)
        {
            IAttackArbiter attackArbiter = new MeleeAttackArbiter(attacker, defender);
            AttackResult meleeAttackResult = DamageTarget(attacker, defender, attackArbiter, attackType);

            if (meleeAttackResult != null)
            {
                _battlePacketFactory.SendMeleeAttack(attacker, attackType, defender.Id, unknwonParam: 0, meleeAttackResult.Flags);
            }
        }

        /// <inheritdoc />
        public void MagicAttack(ILivingEntity attacker, ILivingEntity defender, ObjectMessageType attackType, int magicAttackPower)
        {
            var projectile = new MagicProjectileInfo(attacker, defender, magicAttackPower, onArrived: () =>
            {
                IAttackArbiter magicAttackArbiter = new MagicAttackArbiter(attacker, defender, magicAttackPower);

                DamageTarget(attacker, defender, magicAttackArbiter, ObjectMessageType.OBJMSG_ATK_MAGIC1);
            });
            int projectileId = _projectileSystem.CreateProjectile(projectile);

            _battlePacketFactory.SendMagicAttack(attacker, ObjectMessageType.OBJMSG_ATK_MAGIC1, defender.Id, magicAttackPower, projectileId);
        }

        /// <inheritdoc />
        public void RangeAttack(ILivingEntity attacker, ILivingEntity defender, ObjectMessageType attackType, int power)
        {
            if (attacker is IPlayerEntity player)
            {
                InventoryItem equipedItem = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon);

                if (equipedItem == null || equipedItem.Data.WeaponType != WeaponType.RANGE_BOW)
                {
                    return;
                }

                InventoryItem bulletItem = player.Inventory.GetEquipedItem(ItemPartType.Bullet);

                if (bulletItem == null || bulletItem.Data.ItemKind3 != ItemKind3.ARROW)
                {
                    return;
                }

                _inventorySystem.DeleteItem(player, bulletItem, 1);
            }

            var projectile = new RangeArrowProjectileInfo(attacker, defender, power, onArrived: () =>
            {
                IAttackArbiter attackArbiter = new MeleeAttackArbiter(attacker, defender);
                
                DamageTarget(attacker, defender, attackArbiter, attackType);
            });
            int projectileId = _projectileSystem.CreateProjectile(projectile);

            _battlePacketFactory.SendRangeAttack(attacker, ObjectMessageType.OBJMSG_ATK_RANGE1, defender.Id, power, projectileId);
        }

        /// <summary>
        /// Check if the attacker entity can attack a defender entity.
        /// </summary>
        /// <param name="attacker">Attacker living entity.</param>
        /// <param name="defender">Defender living entity.</param>
        /// <returns></returns>
        private bool CanAttack(ILivingEntity attacker, ILivingEntity defender)
        {
            if (attacker == defender)
            {
                _logger.LogError($"{attacker} cannot attack itself.");
                return false;
            }

            if (attacker.IsDead)
            {
                _logger.LogError($"{attacker} cannot attack because its dead.");
                return false;
            }

            if (defender.IsDead)
            {
                _logger.LogError($"{attacker} cannot attack {defender} because target is already dead.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the defender has been killed by the attacker.
        /// </summary>
        /// <param name="attacker">Attacker living entity.</param>
        /// <param name="defender">Defender living entity.</param>
        /// <param name="attackType">Killing attack type.</param>
        private void CheckIfDefenderIsDead(ILivingEntity attacker, ILivingEntity defender, ObjectMessageType attackType)
        {
            if (defender.IsDead)
            {
                _logger.LogDebug($"{attacker.Object.Name} killed {defender.Object.Name}.");
                defender.Attributes[DefineAttributes.HP] = 0;
                _moverPacketFactory.SendUpdateAttributes(defender, DefineAttributes.HP, defender.Attributes[DefineAttributes.HP]);
                ClearBattleTargets(defender);
                ClearBattleTargets(attacker);

                if (defender is IMonsterEntity && attacker is IPlayerEntity player)
                {
                    _battlePacketFactory.SendDie(player, defender, attacker, attackType);
                }
                else if (defender is IPlayerEntity && attacker is IPlayerEntity)
                {
                    // TODO: PVP
                }
                else if (defender is IPlayerEntity deadPlayer)
                {
                    _battlePacketFactory.SendDie(deadPlayer, defender, attacker, attackType);
                }

                attacker.Behavior.OnTargetKilled(defender);
                defender.Behavior.OnKilled(attacker);
            }
        }

        /// <summary>
        /// Clears the battle targets.
        /// </summary>
        /// <param name="entity">Current entity.</param>
        private void ClearBattleTargets(ILivingEntity entity)
        {
            entity.Follow.Reset();
            entity.Object.MovingFlags &= ~ObjectState.OBJSTA_FMOVE;
            entity.Object.MovingFlags |= ObjectState.OBJSTA_STAND;
            entity.Moves.DestinationPosition.Reset();

            entity.Battle.Reset();
        }
    }
}

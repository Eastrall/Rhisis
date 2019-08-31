using Rhisis.Core.Data;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IBattlePacketFactory
    { 
        /// <summary>
        /// Sends the damages inflicted to a living entity.
        /// </summary>
        /// <param name="defender">Defender entity.</param>
        /// <param name="attacker">Attacker entity.</param>
        /// <param name="attackFlags">Attack flags.</param>
        /// <param name="damage">Amount of damage.</param>
        void SendAddDamage(ILivingEntity defender, ILivingEntity attacker, AttackFlags attackFlags, int damage);

        /// <summary>
        /// Sends a melee attack motion.
        /// </summary>
        /// <param name="attacker">Attacker entity.</param>
        /// <param name="motion">Attack motion.</param>
        /// <param name="targetId">Target.</param>
        /// <param name="unknwonParam">Unknown parameter from client.</param>
        /// <param name="attackFlags">Attack flags.</param>
        void SendMeleeAttack(ILivingEntity attacker, ObjectMessageType motion, uint targetId, int unknwonParam, AttackFlags attackFlags);

        /// <summary>
        /// Sends a packet that makes the entity die.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="deadEntity">Dead entity.</param>
        /// <param name="killerEntity">Killer entity.</param>
        /// <param name="motion">Motion.</param>
        void SendDie(IPlayerEntity player, ILivingEntity deadEntity, ILivingEntity killerEntity, ObjectMessageType motion);
    }
}
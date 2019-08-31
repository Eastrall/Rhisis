using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    internal sealed class BattlePacketFactory : IBattlePacketFactory
    {
        private readonly IPacketFactoryUtilities _packetFactoryUtilities;

        /// <summary>
        /// Creates a new <see cref="BattlePacketFactory"/> instance.
        /// </summary>
        /// <param name="packetFactoryUtilities">Packet factory utilities.</param>
        public BattlePacketFactory(IPacketFactoryUtilities packetFactoryUtilities)
        {
            this._packetFactoryUtilities = packetFactoryUtilities;
        }

        /// <inheritdoc />
        public void SendAddDamage(ILivingEntity defender, ILivingEntity attacker, AttackFlags attackFlags, int damage)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(defender.Id, SnapshotType.DAMAGE);
                packet.Write(attacker.Id);
                packet.Write(damage);
                packet.Write((int)attackFlags);

                if (attackFlags.HasFlag(AttackFlags.AF_FLYING))
                {
                    packet.Write(defender.Moves.DestinationPosition.X);
                    packet.Write(defender.Moves.DestinationPosition.Y);
                    packet.Write(defender.Moves.DestinationPosition.Z);
                    packet.Write(defender.Object.Angle);
                }

                this._packetFactoryUtilities.SendToVisible(packet, defender, sendToPlayer: true);
            }
        }

        /// <inheritdoc />
        public void SendMeleeAttack(ILivingEntity attacker, ObjectMessageType motion, uint targetId, int unknwonParam, AttackFlags attackFlags)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(attacker.Id, SnapshotType.MELEE_ATTACK);
                packet.Write((int)motion);
                packet.Write(targetId);
                packet.Write(unknwonParam);
                packet.Write((int)attackFlags);

                this._packetFactoryUtilities.SendToVisible(packet, attacker);
            }
        }

        /// <inheritdoc />
        public void SendDie(IPlayerEntity player, ILivingEntity deadEntity, ILivingEntity killerEntity, ObjectMessageType motion)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(deadEntity.Id, SnapshotType.MOVERDEATH);
                packet.Write((int)motion);
                packet.Write(killerEntity.Id);

                this._packetFactoryUtilities.SendToVisible(packet, player, sendToPlayer: true);
            }
        }
    }
}
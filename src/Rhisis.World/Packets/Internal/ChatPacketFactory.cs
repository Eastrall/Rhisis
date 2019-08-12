using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class ChatPacketFactory : IChatPacketFactory
    {
        private readonly IPacketFactoryUtilities _packetFactoryUtilities;

        /// <summary>
        /// Creates a new <see cref="ChatPacketFactory"/> instance.
        /// </summary>
        /// <param name="packetFactoryUtilities">Packet factory utilities.</param>
        public ChatPacketFactory(IPacketFactoryUtilities packetFactoryUtilities)
        {
            this._packetFactoryUtilities = packetFactoryUtilities;
        }

        /// <inheritdoc />
        public void SendChat(IPlayerEntity player, string message)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.CHAT);
                packet.Write(message);

                this._packetFactoryUtilities.SendToVisible(packet, player, sendToPlayer: true);
            }
        }

        /// <inheritdoc />
        public void SendChatTo(IWorldEntity fromEntity, IPlayerEntity toPlayer, string message)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(fromEntity.Id, SnapshotType.CHAT);
                packet.Write(message);

                toPlayer.Connection.Send(packet);
            }
        }
    }
}

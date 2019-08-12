using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class CustomizationPacketFactory : ICustomizationPacketFactory
    {
        private readonly IPacketFactoryUtilities _packetFactoryUtilities;

        /// <summary>
        /// Creates a new <see cref="CustomizationPacketFactory"/> instance.
        /// </summary>
        /// <param name="packetFactoryUtilities">Packet Factory utilities.</param>
        public CustomizationPacketFactory(IPacketFactoryUtilities packetFactoryUtilities)
        {
            this._packetFactoryUtilities = packetFactoryUtilities;
        }

        /// <inheritdoc />
        public void SendChangeFace(IPlayerEntity entity, uint faceId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.CHANGEFACE);
                packet.Write(entity.PlayerData.Id);
                packet.Write(faceId);

                this._packetFactoryUtilities.SendToVisible(packet, entity, true);
            }
        }

        /// <inheritdoc />
        public void SendSetHair(IPlayerEntity entity, byte hairId, byte r, byte g, byte b)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SET_HAIR);
                packet.Write(hairId);
                packet.Write(r);
                packet.Write(g);
                packet.Write(b);

                this._packetFactoryUtilities.SendToVisible(packet, entity, true);
            }
        }
    }
}

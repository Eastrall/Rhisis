using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    internal sealed class TextPacketFactory : ITextPacketFactory
    {
        /// <inheritdoc />
        public void SendDefinedText(IPlayerEntity player, DefineText textId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.DEFINEDTEXT1);
                packet.Write((int)textId);
                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendDefinedText(IPlayerEntity player, DefineText textId, params object[] parameters)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.DEFINEDTEXT);
                packet.Write((int)textId);
                packet.Write(string.Join(" ", parameters));
                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendSnoop(IPlayerEntity player, string text)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(0, SnapshotType.SNOOP);
                packet.Write(text);

                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendFeatureNotImplemented(IPlayerEntity player, string feature) => SendSnoop(player, $"Not implemented: {feature}");

        /// <inheritdoc />
        public void SendWorldMessage(IPlayerEntity entity, string text)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.WORLDMSG, 0xFFFFFF00);
                packet.Write(text);

                entity.Connection.Send(packet);
            }
        }
    }
}

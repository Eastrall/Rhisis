using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class NpcBuffPacketFactory : INpcBuffPacketFactory
    {
        /// <inheritdoc />
        public void NpcGivesBuff(IPlayerEntity player, string buff)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETSKILLSTATE);

                player.Connection.Send(packet);
            }
        }
    }
}

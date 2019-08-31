using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    internal sealed class TaskbarPacketFactory : ITaskbarPacketFactory
    {
        /// <inheritdoc />
        public void SendSetActionPoint(IPlayerEntity player, int actionPoint)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETACTIONPOINT);
                packet.Write(actionPoint);

                player.Connection.Send(packet);
            }
        }
    }
}
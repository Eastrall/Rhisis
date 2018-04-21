using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSay(IPlayerEntity player, int targetId, string privatemessage)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(targetId, SnapshotType.RETURNSAY);
                packet.Write(privatemessage);


                player.Connection.Send(packet);
            }
        }
    }
}
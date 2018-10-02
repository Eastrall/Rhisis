using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.Statistics;

namespace Rhisis.World.Handlers
{
    public static class StatisticsHandler
    {
        [PacketHandler(PacketType.MODIFY_STATUS)]
        public static void OnModifyStatus(WorldClient client, INetPacketStream packet)
        {
            var strenght = (ushort)packet.Read<int>();
            var stamina = (ushort)packet.Read<int>();
            var dexterity = (ushort)packet.Read<int>();
            var intelligence = (ushort)packet.Read<int>();
            var statisticsEventArgs = new StatisticsModifyEventArgs(strenght, stamina, dexterity, intelligence);

            client.Player.NotifySystem<StatisticsSystem>(statisticsEventArgs);
        }
    }
}

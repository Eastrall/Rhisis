using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Statistics;

namespace Rhisis.World.Handlers
{
    public static class StatisticsHandler
    {
        [PacketHandler(PacketType.MODIFY_STATUS)]
        public static void OnModifyStatus(WorldClient client, INetPacketStream packet)
        {
            var modifyStatusPacket = new ModifyStatusPacket(packet);
            var statisticsEventArgs = new StatisticsModifyEventArgs(modifyStatusPacket.Strenght,
                modifyStatusPacket.Stamina,
                modifyStatusPacket.Dexterity,
                modifyStatusPacket.Intelligence);

            SystemManager.Instance.Execute<StatisticsSystem>(client.Player, statisticsEventArgs);
        }
    }
}

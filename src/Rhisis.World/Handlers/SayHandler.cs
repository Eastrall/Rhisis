using Ether.Network.Packets;
using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using Rhisis.Core.Network.Packets.World;
using Rhisis.World.Systems.Chat;
using Rhisis.World.Systems.Say;

namespace Rhisis.World.Handlers
{
    public static class SayHandler
    {
        [PacketHandler(PacketType.SAY)]
        public static void OnSay(WorldClient client, INetPacketStream packet)
        {
            var SayPacket = new SayPacket(packet);
            var SayPacketTwo = new SayPacketTwo(packet);
            var SayEvent = new SayEventArgsTwo(SayPacketTwo.PrivateMessage);
            var SayEvent2 = new SayEventArgs(SayPacket.TargetSayId);

            client.Player.Context.NotifySystem<SaySystem>(client.Player, SayEvent);
            client.Player.Context.NotifySystem<SaySystem>(client.Player, SayEvent2);
        }
    }
}

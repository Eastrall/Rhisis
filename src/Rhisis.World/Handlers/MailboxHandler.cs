using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World.Mailbox;
using Rhisis.World.Systems.Mailbox;

namespace Rhisis.World.Handlers
{
    public static class MailboxHandler
    {
        [PacketHandler(PacketType.QUERYMAILBOX)]
        public static void OnQueryMailbox(WorldClient client, INetPacketStream packet)
        {
            var onQueryMailboxPacket = new QueryMailboxPacket(packet);
            var queryMailboxEvent = new QueryMailboxEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryMailboxEvent);
        }
    }
}

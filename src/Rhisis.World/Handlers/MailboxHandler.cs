using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World.Mailbox;
using Rhisis.World.Systems.Mailbox;
using Rhisis.World.Systems.Mailbox.EventArgs;

namespace Rhisis.World.Handlers
{
    public static class MailboxHandler
    {
        /// <summary>
        /// Show all mails.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [PacketHandler(PacketType.QUERYMAILBOX)]
        public static void OnQueryMailbox(WorldClient client, INetPacketStream packet)
        {
            var onQueryMailboxPacket = new QueryMailboxPacket(packet);
            var queryMailboxEvent = new QueryMailboxEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryMailboxEvent);
        }

        /// <summary>
        /// Send a mail.
        /// </summary>
        /// <param name="client"></param>
        /// <param name=""></param>
        [PacketHandler(PacketType.QUERYPOSTMAIL)]
        public static void OnQueryPostMail(WorldClient client, INetPacketStream packet)
        {
            var onQueryPostMailPacket = new QueryPostMailPacket(packet);
            var queryPostMailEvent = new QueryPostMailEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryPostMailEvent);
        }

        /// <summary>
        /// Delete a mail.
        /// </summary>
        /// <param name="client"></param>
        /// <param name=""></param>
        [PacketHandler(PacketType.QUERYREMOVEMAIL)]
        public static void OnQueryRemoveMail(WorldClient client, INetPacketStream packet)
        {
            var onQueryRemoveMailPacket = new QueryRemoveMailPacket(packet);
            var queryRemoveMailEvent = new QueryRemoveMailEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryRemoveMailEvent);
        }

        /// <summary>
        /// Get an item out of a mail.
        /// </summary>
        /// <param name="client"></param>
        /// <param name=""></param>
        [PacketHandler(PacketType.QUERYGETMAILITEM)]
        public static void OnQueryGetMailItem(WorldClient client, INetPacketStream packet)
        {
            var onQueryGetMailItem = new QueryGetMailItemPacket(packet);
            var queryGetMailItemEvent = new QueryGetMailItemEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryGetMailItemEvent);
        }

        /// <summary>
        /// Get an item out of a mail.
        /// </summary>
        /// <param name="client"></param>
        /// <param name=""></param>
        [PacketHandler(PacketType.QUERYGETMAILGOLD)]
        public static void OnQueryGetMailGold(WorldClient client, INetPacketStream packet)
        {
            var onQueryGetMailGold = new QueryGetMailGoldPacket(packet);
            var queryGetMailGoldEvent = new QueryGetMailGoldEventArgs();
            client.Player.NotifySystem<MailboxSystem>(queryGetMailGoldEvent);
        }

        [PacketHandler(PacketType.READMAIL)]
        public static void OnReadMail(WorldClient client, INetPacketStream packet)
        {
            var onReadMail = new ReadMailPacket(packet);
            var readMailEvent = new ReadMailEventArgs();
            client.Player.NotifySystem<MailboxSystem>(readMailEvent);
        }
    }
}

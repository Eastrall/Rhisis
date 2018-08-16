using Ether.Network.Packets;
using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using Rhisis.World.Systems.Messenger;
using Rhisis.World.Systems.Messenger.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class MessengerHandler
    {
        [PacketHandler(PacketType.ADDFRIENDREQEST)]
        public static void OnAddFriendRequest(WorldClient client, INetPacketStream packet)
        {
            var senderId = packet.Read<int>();
            var receiverId = packet.Read<int>();
            var addFriendRequestEventArgs = new AddFriendRequestEventArgs(senderId, receiverId);

            client.Player.NotifySystem<MessengerSystem>(addFriendRequestEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIENDCANCEL)]
        public static void OnAddFriendCancel(WorldClient client, INetPacketStream packet)
        {
            var senderId = packet.Read<int>();
            var receiverId = packet.Read<int>();
            var addFriendCancelEventArgs = new AddFriendCancelEventArgs(senderId, receiverId);

            client.Player.NotifySystem<MessengerSystem>(addFriendCancelEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIENDNAMEREQEST)]
        public static void OnAddFriendNameRequest(WorldClient client, INetPacketStream packet)
        {
            var senderId = packet.Read<int>();
            var receiverId = packet.Read<string>();
            var addFriendNameRequestEventArgs = new AddFriendNameRequestEventArgs(senderId, receiverId);

            client.Player.NotifySystem<MessengerSystem>(addFriendNameRequestEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIEND)]
        public static void OnAddFriend(WorldClient client, INetPacketStream packet)
        {
            var receiverId = packet.Read<int>();
            var senderId = packet.Read<int>();
            var receiverGender = packet.Read<byte>();
            var senderGender = packet.Read<byte>();
            var receiverJob = packet.Read<int>();
            var senderJob = packet.Read<int>();
            var addFriendEventArgs = new AddFriendEventArgs(senderId, receiverId, senderGender, receiverGender, senderJob, receiverJob);

            client.Player.NotifySystem<MessengerSystem>(addFriendEventArgs);
        }
    }
}
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
            var leaderId = packet.Read<int>();
            var memberId = packet.Read<int>();
            var addFriendRequestEventArgs = new AddFriendRequestEventArgs(leaderId, memberId);

            client.Player.NotifySystem<MessengerSystem>(addFriendRequestEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIENDCANCEL)]
        public static void OnAddFriendCancel(WorldClient client, INetPacketStream packet)
        {
            var leaderId = packet.Read<int>();
            var memberId = packet.Read<int>();
            var addFriendCancelEventArgs = new AddFriendCancelEventArgs(leaderId, memberId);

            client.Player.NotifySystem<MessengerSystem>(addFriendCancelEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIENDNAMEREQEST)]
        public static void OnAddFriendNameRequest(WorldClient client, INetPacketStream packet)
        {
            var leaderId = packet.Read<int>();
            var memberName = packet.Read<string>();
            var addFriendNameRequestEventArgs = new AddFriendNameRequestEventArgs(leaderId, memberName);

            client.Player.NotifySystem<MessengerSystem>(addFriendNameRequestEventArgs);
        }

        [PacketHandler(PacketType.ADDFRIEND)]
        public static void OnAddFriend(WorldClient client, INetPacketStream packet)
        {
            var memberId = packet.Read<int>();
            var leaderId = packet.Read<int>();
            var memberGender = packet.Read<byte>();
            var leaderGender = packet.Read<byte>();
            var memberJob = packet.Read<int>();
            var leaderJob = packet.Read<int>();
            var addFriendEventArgs = new AddFriendEventArgs(leaderId, memberId, leaderGender, memberGender, leaderJob, memberJob);

            client.Player.NotifySystem<MessengerSystem>(addFriendEventArgs);
        }
    }
}
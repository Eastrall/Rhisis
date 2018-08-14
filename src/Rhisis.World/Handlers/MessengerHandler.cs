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
    }
}
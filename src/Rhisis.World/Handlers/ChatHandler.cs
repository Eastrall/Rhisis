using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.Chat;

namespace Rhisis.World.Handlers
{
    public static class ChatHandler
    {
        [PacketHandler(PacketType.CHAT)]
        public static void OnChat(WorldClient client, INetPacketStream packet)
        {
            var chatMessage = packet.Read<string>();
            var chatEvent = new ChatEventArgs(chatMessage);

            client.Player.NotifySystem<ChatSystem>(chatEvent);
        }
    }
}

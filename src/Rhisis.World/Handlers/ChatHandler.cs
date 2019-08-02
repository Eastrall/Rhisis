using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Chat;

namespace Rhisis.World.Handlers
{
    public static class ChatHandler
    {
        [PacketHandler(PacketType.CHAT)]
        public static void OnChat(WorldClient client, INetPacketStream packet)
        {
            var chatPacket = new ChatPacket(packet);
            var chatEvent = new ChatEventArgs(chatPacket.Message);

            SystemManager.Instance.Execute<ChatSystem>(client.Player, chatEvent);
        }
    }
}

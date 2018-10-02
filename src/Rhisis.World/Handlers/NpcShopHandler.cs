using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.NpcShop;
using Rhisis.World.Systems.NpcShop.EventArgs;

namespace Rhisis.World.Handlers
{
    public static class NpcShopHandler
    {
        [PacketHandler(PacketType.OPENSHOPWND)]
        public static void OnOpenShopWindow(WorldClient client, INetPacketStream packet)
        {
            var objectId = packet.Read<int>();
            var npcEvent = new NpcShopOpenEventArgs(objectId);

            client.Player.NotifySystem<NpcShopSystem>(npcEvent);
        }

        [PacketHandler(PacketType.CLOSESHOPWND)]
        public static void OnCloseShopWindow(WorldClient client, INetPacketStream packet)
        {
            client.Player.NotifySystem<NpcShopSystem>(new NpcShopCloseEventArgs());
        }

        [PacketHandler(PacketType.BUYITEM)]
        public static void OnBuyItem(WorldClient client, INetPacketStream packet)
        {
            var tab = packet.Read<byte>();
            var slot = packet.Read<byte>();
            var quantity = packet.Read<short>();
            var itemId = packet.Read<int>();
            var npcShopEvent = new NpcShopBuyEventArgs(itemId, quantity, tab, slot);

            client.Player.NotifySystem<NpcShopSystem>(npcShopEvent);
        }

        [PacketHandler(PacketType.SELLITEM)]
        public static void OnSellItem(WorldClient client, INetPacketStream packet)
        {
            var itemUniqueId = packet.Read<byte>();
            var quantity = packet.Read<short>();
            var npcShopEvent = new NpcShopSellEventArgs(itemUniqueId, quantity);

            client.Player.NotifySystem<NpcShopSystem>(npcShopEvent);
        }
    }
}

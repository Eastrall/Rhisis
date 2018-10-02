using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.Trade;
using Rhisis.World.Systems.Trade.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class TradeHandler
    {
        [PacketHandler(PacketType.CONFIRMTRADE)]
        public static void OnTradeRequest(WorldClient client, INetPacketStream packet)
        {
            var targetId = packet.Read<int>();
            var tradeEvent = new TradeRequestEventArgs(targetId);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.CONFIRMTRADECANCEL)]
        public static void OnTradeRequestCancel(WorldClient client, INetPacketStream packet)
        {
            var targetId = packet.Read<int>();
            var tradeEvent = new TradeRequestCancelEventArgs(targetId);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.TRADE)]
        public static void OnTrade(WorldClient client, INetPacketStream packet)
        {
            var targetId = packet.Read<int>();
            var tradeEvent = new TradeBeginEventArgs(targetId);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.TRADEPUT)]
        public static void OnTradePut(WorldClient client, INetPacketStream packet)
        {
            var position = packet.Read<byte>();
            var itemType = packet.Read<byte>();
            var itemId = packet.Read<byte>();
            var count = packet.Read<short>();
            var tradeEvent = new TradePutEventArgs(position, itemType, itemId, count);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.TRADEPUTGOLD)]
        public static void OnTradePutGold(WorldClient client, INetPacketStream packet)
        {
            var gold = packet.Read<int>();
            var tradeEvent = new TradePutGoldEventArgs(gold);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.TRADECANCEL)]
        public static void OnTradeCancel(WorldClient client, INetPacketStream packet)
        {
            var mode = packet.Read<int>();
            var tradeEvent = new TradeCancelEventArgs(mode);

            client.Player.NotifySystem<TradeSystem>(tradeEvent);
        }

        [PacketHandler(PacketType.TRADEOK)]
        public static void OnTradeOk(WorldClient client, INetPacketStream packet)
        {
            client.Player.NotifySystem<TradeSystem>(new TradeOkEventArgs());
        }

        [PacketHandler(PacketType.TRADECONFIRM)]
        public static void OnTradeConfirm(WorldClient client, INetPacketStream packet)
        {
            client.Player.NotifySystem<TradeSystem>(new TradeConfirmEventArgs());
        }
    }
}

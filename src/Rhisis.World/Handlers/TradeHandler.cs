using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World.Trade;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Trade;
using Rhisis.World.Systems.Trade.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class TradeHandler
    {
        [PacketHandler(PacketType.CONFIRMTRADE)]
        public static void OnTradeRequest(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradeRequestPacket(packet);
            var tradeEvent = new TradeRequestEventArgs(tradePacket.Target);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.CONFIRMTRADECANCEL)]
        public static void OnTradeRequestCancel(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradeRequestPacket(packet);
            var tradeEvent = new TradeRequestCancelEventArgs(tradePacket.Target);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.TRADE)]
        public static void OnTrade(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradeRequestPacket(packet);
            var tradeEvent = new TradeBeginEventArgs(tradePacket.Target);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.TRADEPUT)]
        public static void OnTradePut(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradePutPacket(packet);
            var tradeEvent = new TradePutEventArgs(tradePacket.Position, tradePacket.ItemType, tradePacket.ItemId,
                tradePacket.Count);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.TRADEPUTGOLD)]
        public static void OnTradePutGold(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradePutGoldPacket(packet);
            var tradeEvent = new TradePutGoldEventArgs(tradePacket.Gold);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.TRADECANCEL)]
        public static void OnTradeCancel(WorldClient client, INetPacketStream packet)
        {
            var tradePacket = new TradeCancelPacket(packet);
            var tradeEvent = new TradeCancelEventArgs(tradePacket.Mode);

            SystemManager.Instance.Execute<TradeSystem>(client.Player, tradeEvent);
        }

        [PacketHandler(PacketType.TRADEOK)]
        public static void OnTradeOk(WorldClient client, INetPacketStream packet)
        {
            SystemManager.Instance.Execute<TradeSystem>(client.Player, new TradeOkEventArgs());
        }

        [PacketHandler(PacketType.TRADECONFIRM)]
        public static void OnTradeConfirm(WorldClient client, INetPacketStream packet)
        {
            SystemManager.Instance.Execute<TradeSystem>(client.Player, new TradeConfirmEventArgs());
        }
    }
}

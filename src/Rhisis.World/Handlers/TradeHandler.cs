using Ether.Network.Packets;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World.Trade;
using Rhisis.World.Client;
using Rhisis.World.Systems.Trade;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class TradeHandler
    {
        private readonly ITradeSystem _tradeSystem;

        public TradeHandler(ITradeSystem tradeSystem)
        {
            this._tradeSystem = tradeSystem;
        }

        [HandlerAction(PacketType.CONFIRMTRADE)]
        public void OnTradeRequest(IWorldClient client, TradeRequestPacket packet)
        {
            this._tradeSystem.RequestTrade(client.Player, packet.TargetId);
        }

        [HandlerAction(PacketType.CONFIRMTRADECANCEL)]
        public void OnTradeRequestCancel(IWorldClient client, TradeRequestPacket packet)
        {
            this._tradeSystem.DeclineTradeRequest(client.Player, packet.TargetId);
        }

        [HandlerAction(PacketType.TRADE)]
        public void OnTrade(IWorldClient client, TradeRequestPacket packet)
        {
            this._tradeSystem.StartTrade(client.Player, packet.TargetId);
        }

        [HandlerAction(PacketType.TRADEPUT)]
        public void OnTradePut(IWorldClient client, TradePutPacket packet)
        {
            this._tradeSystem.PutItem(client.Player, packet.ItemUniqueId, packet.Count, packet.ItemType, packet.Position);
        }

        [HandlerAction(PacketType.TRADEPUTGOLD)]
        public void OnTradePutGold(IWorldClient client, TradePutGoldPacket packet)
        {
            this._tradeSystem.PutGold(client.Player, packet.Gold);
        }

        [HandlerAction(PacketType.TRADECANCEL)]
        public void OnTradeCancel(IWorldClient client, TradeCancelPacket packet)
        {
            this._tradeSystem.CancelTrade(client.Player, packet.Mode);
        }

        [HandlerAction(PacketType.TRADEOK)]
        public void OnTradeOk(IWorldClient client)
        {
            this._tradeSystem.ConfirmTrade(client.Player);
        }

        [HandlerAction(PacketType.TRADECONFIRM)]
        public void OnTradeConfirm(IWorldClient client)
        {
            this._tradeSystem.LastConfirmTrade(client.Player);
        }
    }
}

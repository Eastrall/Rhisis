using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface ITradePacketFactory
    {
        void SendTrade(IPlayerEntity player, IPlayerEntity target, uint playerId);
        void SendTradeCancel(IPlayerEntity player, int mode);
        void SendTradeConsent(IPlayerEntity player);
        void SendTradeLastConfirm(IPlayerEntity player);
        void SendTradeLastConfirmOk(IPlayerEntity player, uint traderId);
        void SendTradeOk(IPlayerEntity player, uint traderId);
        void SendTradePut(IPlayerEntity player, IPlayerEntity trader, byte slot, byte itemType, byte id, short count);

        void SendTradePutError(IPlayerEntity player);

        void SendTradePutGold(IPlayerEntity player, IPlayerEntity trader, int gold);
        void SendTradeRequest(IPlayerEntity player, IPlayerEntity target);
        void SendTradeRequestCancel(IPlayerEntity player, IPlayerEntity target);
    }
}
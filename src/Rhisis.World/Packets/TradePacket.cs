using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendTradeRequest(IPlayerEntity player, uint traderId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.CONFIRMTRADE);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeRequestCancel(IPlayerEntity player, uint traderId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.CONFIRMTRADECANCEL);

                player.Connection.Send(packet);
            }
        }

        public static void SendTrade(IPlayerEntity player, IPlayerEntity target, uint playerId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(target.Id, SnapshotType.TRADE);

                packet.Write(playerId);

                target.Inventory.Serialize(packet);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradePut(IPlayerEntity player, uint traderId, byte slot, byte itemType, byte id, short count)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.TRADEPUT);

                packet.Write(slot);
                packet.Write(itemType);
                packet.Write(id);
                packet.Write(count);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradePutGold(IPlayerEntity player, uint traderId, int gold)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.TRADEPUTGOLD);

                packet.Write(gold);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeCancel(IPlayerEntity player, uint traderId, int mode)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.TRADECANCEL);

                packet.Write(traderId);
                packet.Write(mode);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeOk(IPlayerEntity player, uint traderId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.TRADEOK);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeLastConfirm(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(0, SnapshotType.TRADELASTCONFIRM);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeLastConfirmOk(IPlayerEntity player, uint traderId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(traderId, SnapshotType.TRADELASTCONFIRMOK);

                player.Connection.Send(packet);
            }
        }

        public static void SendTradeConsent(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(0, SnapshotType.TRADECONSENT);

                player.Connection.Send(packet);
            }
        }
    }
}

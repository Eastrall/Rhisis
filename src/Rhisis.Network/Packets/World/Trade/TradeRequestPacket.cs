using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Trade
{
    public sealed class TradeRequestPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the target object id for trade.
        /// </summary>
        public uint TargetId { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.TargetId = packet.Read<uint>();
        }
    }
}

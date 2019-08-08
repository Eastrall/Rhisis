using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    public class BuyItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the tab.
        /// </summary>
        public byte Tab { get; set; }

        /// <summary>
        /// Gets the slot of the item.
        /// </summary>
        public byte Slot { get; set; }

        /// <summary>
        /// Gets the quantity of items.
        /// </summary>
        public short Quantity { get; set; }

        /// <summary>
        /// Gets the Item Id.
        /// </summary>
        public int ItemId { get; set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.Tab = packet.Read<byte>();
            this.Slot = packet.Read<byte>();
            this.Quantity = packet.Read<short>();
            this.ItemId = packet.Read<int>();
        }
    }
}

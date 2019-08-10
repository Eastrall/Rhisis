using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="MoveItemPacket"/> structure.
    /// </summary>
    public class MoveItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the Item type.
        /// </summary>
        public byte ItemType { get; private set; }

        /// <summary>
        /// Gets the Item source slot.
        /// </summary>
        public byte SourceSlot { get; private set; }

        /// <summary>
        /// Gets the item destination slot.
        /// </summary>
        public byte DestinationSlot { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.ItemType = packet.Read<byte>();
            this.SourceSlot = packet.Read<byte>();
            this.DestinationSlot = packet.Read<byte>();
        }
    }
}

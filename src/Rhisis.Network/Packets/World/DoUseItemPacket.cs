using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Describes the packet structure of the <see cref="PacketType.DOUSEITEM"/>.
    /// </summary>
    public class DoUseItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets or sets the Unique Item Id.
        /// </summary>
        public int UniqueItemId { get; private set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public int ObjectId { get; private set; }

        /// <summary>
        /// Gets or sets the body part.
        /// </summary>
        public int Part { get; private set; }

        /// <summary>
        /// Gets or sets the item flight speed.
        /// </summary>
        public float FlightSpeed { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.UniqueItemId = (packet.Read<int>() >> 16) & 0xFFFF;
            this.ObjectId = packet.Read<int>();
            this.Part = packet.Read<int>();
            this.FlightSpeed = packet.Read<float>();
        }
    }
}
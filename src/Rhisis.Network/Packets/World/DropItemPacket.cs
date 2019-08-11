using Ether.Network.Packets;
using Rhisis.Core.Structures;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="DropItemPacket"/> structure.
    /// </summary>
    public class DropItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the item type
        /// </summary>
        public uint ItemType { get; private set; }

        /// <summary>
        /// Gets the unique item id.
        /// </summary>
        public int ItemUniqueId { get; private set; }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int ItemQuantity { get; private set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <inhertidoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.ItemType = packet.Read<uint>();
            this.ItemUniqueId = packet.Read<int>();
            this.ItemQuantity = packet.Read<short>();
            this.Position = new Vector3(packet.Read<float>(), packet.Read<float>(), packet.Read<float>());
        }
    }
}
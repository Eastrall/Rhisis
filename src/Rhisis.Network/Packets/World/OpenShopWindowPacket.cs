using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="OpenShopWindowPacket"/> packet structure.
    /// </summary>
    public class OpenShopWindowPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the selected object id.
        /// </summary>
        public uint ObjectId { get; set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.ObjectId = packet.Read<uint>();
        }
    }
}

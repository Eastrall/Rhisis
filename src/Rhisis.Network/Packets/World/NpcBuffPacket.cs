using Sylver.Network.Data;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="NpcBuffPacket"/> packet structure.
    /// </summary>
    public class NpcBuffPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the selected buff string.
        /// </summary>
        public string buff { get; set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.buff = packet.Read<string>();
        }
    }
}

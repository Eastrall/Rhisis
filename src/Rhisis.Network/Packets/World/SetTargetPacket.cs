using Ether.Network.Packets;
using Rhisis.Core.Common;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="SetTargetPacket"/> structure.
    /// </summary>
    public class SetTargetPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the target id.
        /// </summary>
        public uint TargetId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether target should be cleared or not.
        /// </summary>
        public TargetModeType TargetMode { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.TargetId = packet.Read<uint>();
            this.TargetMode = (TargetModeType)packet.Read<byte>();
        }
    }
}
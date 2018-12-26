using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="SetTargetPacket"/> structure.
    /// </summary>
    public struct SetTargetPacket : IEquatable<SetTargetPacket>
    {
        /// <summary>
        /// Gets the target id.
        /// </summary>
        public uint TargetId { get; }

        /// <summary>
        /// Gets a value indicating whether target should be cleared or not.
        /// </summary>
        public byte Clear { get; }

        /// <summary>
        /// Creates a new <see cref="SetTargetPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public SetTargetPacket(INetPacketStream packet)
        {
            this.TargetId = packet.Read<uint>();
            this.Clear = packet.Read<byte>();
        }

        /// <summary>
        /// Compares two <see cref="SetTargetPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="SetTargetPacket"/></param>
        /// <returns></returns>
        public bool Equals(SetTargetPacket other) => this.TargetId == other.TargetId && this.Clear == other.Clear;
    }
}
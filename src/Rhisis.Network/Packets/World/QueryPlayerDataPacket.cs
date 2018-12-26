using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="QueryPlayerDataPacket"/> packet structure.
    /// </summary>
    public struct QueryPlayerDataPacket : IEquatable<QueryPlayerDataPacket>
    {
        /// <summary>
        /// Gets the player id.
        /// </summary>
        public uint PlayerId { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Creates a new <see cref="QueryPlayerDataPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryPlayerDataPacket(INetPacketStream packet)
        {
            this.PlayerId = packet.Read<uint>();
            this.Version = packet.Read<int>();
        }

        /// <summary>
        /// Compare two <see cref="QueryPlayerDataPacket"/> instances.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(QueryPlayerDataPacket other) => this.PlayerId == other.PlayerId && this.Version == other.Version;
    }
}

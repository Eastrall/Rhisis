using Ether.Network.Packets;
using System;
using System.Collections.Generic;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="QueryPlayerData2Packet"/> packet structure.
    /// </summary>
    public struct QueryPlayerData2Packet : IEquatable<QueryPlayerData2Packet>
    {
        /// <summary>
        /// Gets the size of the list.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets the player id and version.
        /// </summary>
        public Dictionary<uint, int> PlayerDictionary { get; }

        /// <summary>
        /// Creates a new <see cref="QueryPlayerData2Packet"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryPlayerData2Packet(INetPacketStream packet)
        {
            PlayerDictionary = new Dictionary<uint, int>();
            this.Size = packet.Read<uint>();
            for (uint i = 0; i < this.Size; i++)
                PlayerDictionary.Add(packet.Read<uint>(), packet.Read<int>());
        }

        /// <summary>
        /// Compare two <see cref="QueryPlayerData2Packet"/> instances.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(QueryPlayerData2Packet other) => this.Size == other.Size && this.PlayerDictionary == other.PlayerDictionary;
    }
}

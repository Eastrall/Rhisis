using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public struct QueryMailboxPacket : IEquatable<QueryMailboxPacket>
    {
        /// <summary>
        /// Creates a new <see cref="QueryMailboxPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryMailboxPacket(INetPacketStream packet)
        {
        }

        /// <summary>
        /// Compares two <see cref="QueryMailboxPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="QueryMailboxPacket"/></param>
        /// <returns></returns>
        public bool Equals(QueryMailboxPacket other)
        {
            throw new NotImplementedException();
        }
    }
}

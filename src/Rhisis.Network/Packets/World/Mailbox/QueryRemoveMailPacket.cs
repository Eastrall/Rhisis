using System;
using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public struct QueryRemoveMailPacket : IEquatable<QueryRemoveMailPacket>
    {
        /// <summary>
        /// Gets the id of the mail.
        /// </summary>
        public int MailId { get; }

        /// <summary>
        /// Creates a new <see cref="QueryRemoveMailPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryRemoveMailPacket(INetPacketStream packet)
        {
            this.MailId = packet.Read<int>();
        }

        /// <summary>
        /// Compares two <see cref="QueryRemoveMailPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="QueryRemoveMailPacket"/></param>
        /// <returns></returns>
        public bool Equals(QueryRemoveMailPacket other)
        {
            throw new NotImplementedException();
        }
    }
}

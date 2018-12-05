using System;
using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public struct QueryGetMailItemPacket : IEquatable<QueryGetMailItemPacket>
    {
        /// <summary>
        /// Gets the id of the mail
        /// </summary>
        public int MailId { get; }

        /// <summary>
        /// Creates a new <see cref="QueryGetMailItemPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryGetMailItemPacket(INetPacketStream packet)
        {
            this.MailId = packet.Read<int>();
        }

        /// <summary>
        /// Compares two <see cref="QueryGetMailItemPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="QueryGetMailItemPacket"/></param>
        /// <returns></returns>
        public bool Equals(QueryGetMailItemPacket other)
        {
            throw new NotImplementedException();
        }
    }
}

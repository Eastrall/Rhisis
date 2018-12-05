using System;
using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public struct ReadMailPacket : IEquatable<ReadMailPacket>
    {
        /// <summary>
        /// Gets the id of the mail
        /// </summary>
        public int MailId { get; }

        /// <summary>
        /// Creates a new <see cref="ReadMailPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public ReadMailPacket(INetPacketStream packet)
        {
            this.MailId = packet.Read<int>();
        }

        /// <summary>
        /// Compares two <see cref="ReadMailPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="ReadMailPacket"/></param>
        /// <returns></returns>
        public bool Equals(ReadMailPacket other)
        {
            throw new NotImplementedException();
        }
    }
}

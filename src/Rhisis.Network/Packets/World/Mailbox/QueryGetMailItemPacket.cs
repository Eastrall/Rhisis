﻿using Rhisis.Game.Abstractions.Protocol;
using LiteNetwork.Protocol.Abstractions;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public class QueryGetMailItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the id of the mail
        /// </summary>
        public int MailId { get; private set; }

        /// <inheritdoc />
        public void Deserialize(ILitePacketStream packet)
        {
            MailId = packet.Read<int>();
        }
    }
}

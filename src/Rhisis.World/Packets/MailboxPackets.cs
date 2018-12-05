using Ether.Network.Common;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendMailbox(IPlayerEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.QUERYMAILBOX);

                packet.Write(entity.PlayerData.Id); // character id
                packet.Write(0); // number of mails

                // foreach mail
                // packet.Write(number); // number of mail

                entity.Connection.Send(packet);
            }
        }
    }
}

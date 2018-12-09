using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System.Collections.Generic;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        private const byte CONTAINS_NO_ITEM = 0x0;
        private const byte CONTAINS_ITEM = 0x1;

        public static void SendMailbox(IPlayerEntity entity, ICollection<DbMail> mails)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.QUERYMAILBOX);

                packet.Write(entity.PlayerData.Id); // character id
                packet.Write(mails.Count); // number of mails

                foreach (var mail in mails)
                {
                    packet.Write(mail.Id);
                    packet.Write(mail.Sender.Id);
                    if (mail.Item != null)
                    {
                        packet.Write(CONTAINS_ITEM);
                        var item = new Item(mail.Item);
                        item.Serialize(packet);
                    }
                    else
                        packet.Write(CONTAINS_NO_ITEM);
                    packet.Write(mail.Gold);
                    packet.Write(1); // time
                    packet.Write(mail.HasBeenRead);
                    packet.Write(mail.Title);
                    packet.Write(mail.Text);
                }
                entity.Connection.Send(packet);
            }
        }

        public static void SendQueryPostMail(IPlayerEntity entity, DbMail mail)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.QUERYPOSTMAIL);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Sender.Id);
                if (mail.Item is null)
                    packet.Write((byte)0);
                else
                {
                    packet.Write((byte)1);
                    var item = new Item(mail.Item);
                    item.Serialize(packet);
                }
                packet.Write(mail.Gold);
                packet.Write(mail.Title);
                packet.Write(mail.Text);

                entity.Connection.Send(packet);
            }
        }
    }
}

using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System;
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

                packet.Write((uint)entity.PlayerData.Id);
                packet.Write(mails.Count);

                foreach (var mail in mails)
                {
                    packet.Write((uint)mail.Id);
                    packet.Write((uint)mail.Sender.Id);
                    if (mail.Item is null)
                        packet.Write(CONTAINS_NO_ITEM);
                    else
                    {
                        packet.Write(CONTAINS_ITEM);
                        var item = new Item(mail.Item);
                        item.Serialize(packet);
                    }
                    packet.Write(mail.Gold);
                    int time = (int)(DateTime.UtcNow - mail.CreateTime).TotalSeconds;
                    packet.Write(time);
                    packet.Write(Convert.ToByte(mail.HasBeenRead));
                    packet.Write(mail.Title);
                    packet.Write(mail.Text);
                }
                entity.Connection.Send(packet);
            }
        }

        public static void SendPostMail(IPlayerEntity entity, DbMail mail)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.QUERYPOSTMAIL);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Sender.Id);
                if (mail.Item is null)
                    packet.Write(CONTAINS_NO_ITEM);
                else
                {
                    packet.Write(CONTAINS_ITEM);
                    var item = new Item(mail.Item);
                    item.Serialize(packet);
                }
                packet.Write(mail.Gold);
                packet.Write(mail.Title);
                packet.Write(mail.Text);

                entity.Connection.Send(packet);
            }
        }

        public static void SendRemoveMail(IPlayerEntity entity, DbMail mail)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.QUERYREMOVEMAIL);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Id);

                entity.Connection.Send(packet);
            }
        }

        public static void SendGetMailItem(IPlayerEntity entity, DbMail mail, int channelId)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.QUERYGETMAILITEM);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Id);
                packet.Write(channelId);

                entity.Connection.Send(packet);
            }
        }

        public static void SendGetMailGold(IPlayerEntity entity, DbMail mail, int channelId)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.QUERYGETMAILGOLD);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Id);
                packet.Write(channelId);

                entity.Connection.Send(packet);
            }
        }

        public static void SendReadMail(IPlayerEntity entity, DbMail mail)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(PacketType.READMAIL);

                packet.Write(mail.Receiver.Id);
                packet.Write(mail.Id);

                entity.Connection.Send(packet);
            }
        }
    }
}

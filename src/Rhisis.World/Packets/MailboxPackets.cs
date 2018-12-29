using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Systems.Mailbox;
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
                    packet.Write(mail.Id);
                    packet.Write(mail.Sender.Id);
                    if (mail.Item is null || mail.HasReceivedItem)
                        packet.Write(CONTAINS_NO_ITEM);
                    else
                    {
                        packet.Write(CONTAINS_ITEM);
                        var item = new Item(mail.Item);
                        item.Serialize(packet);
                    }
                    packet.Write(mail.HasReceivedGold ? 0 : mail.Gold);
                    int time = (int)(DateTime.UtcNow - mail.CreateTime).TotalSeconds;
                    packet.Write(time);
                    packet.Write(Convert.ToByte(mail.HasBeenRead));
                    packet.Write(mail.Title);
                    packet.Write(mail.Text);
                }
                entity.Connection.Send(packet);
            }
        }

        public static void SendRemoveMail(IPlayerEntity entity, DbMail mail, RemovedFromMail obj)
        {
            using (var packet = new FFPacket())
            {
                packet.WriteHeader(SnapshotType.REMOVEMAIL);

                packet.Write(mail.Id);
                packet.Write(obj);

                entity.Connection.Send(packet);
            }
        }
    }
}

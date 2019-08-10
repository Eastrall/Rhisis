using Ether.Network.Packets;
using Rhisis.Core.Data;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendToVisible(INetPacketStream packet, IWorldEntity entity, bool sendToPlayer = false)
        {
            IEnumerable<IPlayerEntity> visiblePlayers = from x in entity.Object.Entities
                                                        where x.Type == WorldEntityType.Player
                                                        select x as IPlayerEntity;

            foreach (IPlayerEntity visiblePlayer in visiblePlayers)
                visiblePlayer.Connection.Send(packet);

            if (sendToPlayer && entity is IPlayerEntity player)
                player.Connection.Send(packet);
        }

        public static void SendFollowTarget(IWorldEntity entity, IWorldEntity targetEntity, float distance)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERSETDESTOBJ);
                packet.Write(targetEntity.Id);
                packet.Write(distance);

                SendToVisible(packet, entity);
            }
        }

        public static void SendUpdateAttributes(IWorldEntity entity, DefineAttributes attribute, int newValue)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOINTPARAM);
                packet.Write((int)attribute);
                packet.Write(newValue);
                
                SendToVisible(packet, entity, true);
            }
        }

        public static void SendWorldMsg(IPlayerEntity entity, string text)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.WORLDMSG, 0xFFFFFF00);
                packet.Write(text);

                entity.Connection.Send(packet);
            }
        }
    }
}

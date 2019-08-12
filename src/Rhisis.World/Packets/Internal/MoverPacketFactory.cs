﻿using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class MoverPacketFactory : IMoverPacketFactory
    {
        private readonly IPacketFactoryUtilities _packetFactoryUtilities;

        public MoverPacketFactory(IPacketFactoryUtilities packetFactoryUtilities)
        {
            this._packetFactoryUtilities = packetFactoryUtilities;
        }

        /// <inheritdoc />
        public void SendMoverMoved(IWorldEntity entity, Vector3 beginPosition, Vector3 destinationPosition, float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption, long tickCount)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERMOVED);
                packet.Write(beginPosition.X);
                packet.Write(beginPosition.Y);
                packet.Write(beginPosition.Z);
                packet.Write(destinationPosition.X);
                packet.Write(destinationPosition.Y);
                packet.Write(destinationPosition.Z);
                packet.Write(angle);
                packet.Write(state);
                packet.Write(stateFlag);
                packet.Write(motion);
                packet.Write(motionEx);
                packet.Write(loop);
                packet.Write(motionOption);
                packet.Write(tickCount);

                this._packetFactoryUtilities.SendToVisible(packet, entity, sendToPlayer: false);
            }
        }

        /// <inheritdoc />
        public void SendDestinationAngle(IWorldEntity entity, bool left)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.DESTANGLE);
                packet.Write(entity.Object.Angle);
                packet.Write(left);

                this._packetFactoryUtilities.SendToVisible(packet, entity);
            }
        }

        /// <inheritdoc />
        public void SendDestinationPosition(IMovableEntity movableEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(movableEntity.Id, SnapshotType.DESTPOS);
                packet.Write(movableEntity.Moves.DestinationPosition.X);
                packet.Write(movableEntity.Moves.DestinationPosition.Y);
                packet.Write(movableEntity.Moves.DestinationPosition.Z);
                packet.Write<byte>(1);

                this._packetFactoryUtilities.SendToVisible(packet, movableEntity);
            }
        }

        /// <inheritdoc />
        public void SendMoverPosition(IWorldEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOS);
                packet.Write(entity.Object.Position.X);
                packet.Write(entity.Object.Position.Y);
                packet.Write(entity.Object.Position.Z);

                this._packetFactoryUtilities.SendToVisible(packet, entity, sendToPlayer: true);
            }
        }
    }
}
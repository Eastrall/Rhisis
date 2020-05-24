using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    internal class MoverPacketFactory : PacketFactoryBase, IMoverPacketFactory
    {
        /// <inheritdoc />
        public void SendMoverMoved(IWorldEntity entity, Vector3 beginPosition, Vector3 destinationPosition, float angle, int state, int stateFlag, int motion, int motionEx, int loop, int motionOption, long tickCount)
        {
            using var packet = new FFPacket();

            packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERMOVED);
            packet.Write(beginPosition.X);
            packet.Write(beginPosition.Y);
            packet.Write(beginPosition.Z);
            packet.Write(destinationPosition.X);
            packet.Write(destinationPosition.Y);
            packet.Write(destinationPosition.Z);
            packet.Write(angle);
            if (entity is IPlayerEntity playerEntity)
            {
                if (playerEntity.PlayerData.Mode.HasFlag(ModeType.TRANSPARENT_MODE)) 
                {
                    packet.Write((int)ObjectState.OBJSTA_STAND);
                }
                else
                {
                    packet.Write(state);
                }
            }
            else 
            {
                packet.Write(state);
            }
            packet.Write(stateFlag);
            packet.Write(motion);
            packet.Write(motionEx);
            packet.Write(loop);
            packet.Write(motionOption);
            packet.Write(tickCount);

            SendToVisible(packet, entity, sendToPlayer: false);
        }

        /// <inheritdoc />
        public void SendMoverBehavior(IWorldEntity entity, Vector3 beginPosition, Vector3 destinationPosition, float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption, long tickCount)
        {
            using var packet = new FFPacket();

            packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERBEHAVIOR);
            packet.Write(beginPosition.X);
            packet.Write(beginPosition.Y);
            packet.Write(beginPosition.Z);
            packet.Write(destinationPosition.X);
            packet.Write(destinationPosition.Y);
            packet.Write(destinationPosition.Z);
            packet.Write(angle);
            if (entity is IPlayerEntity playerEntity)
            {
                if (playerEntity.PlayerData.Mode.HasFlag(ModeType.TRANSPARENT_MODE)) 
                {
                    packet.Write(ObjectState.OBJSTA_STAND);
                }
                else
                {
                    packet.Write(state);
                }
            }
            else 
            {
                packet.Write(state);
            }
            packet.Write(stateFlag);
            packet.Write(motion);
            packet.Write(motionEx);
            packet.Write(loop);
            packet.Write(motionOption);
            packet.Write(tickCount);

            SendToVisible(packet, entity, sendToPlayer: false);
        }

        /// <inheritdoc />
        public void SendDestinationAngle(IWorldEntity entity, bool left)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.DESTANGLE);
            packet.Write(entity.Object.Angle);
            packet.Write(left);

            SendToVisible(packet, entity);
        }

        /// <inheritdoc />
        public void SendDestinationPosition(IMovableEntity movableEntity)
        {
            if (movableEntity is IPlayerEntity playerEntity)
            {
                if (playerEntity.PlayerData.Mode.HasFlag(ModeType.TRANSPARENT_MODE)) 
                {
                    return;
                }
            }

            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(movableEntity.Id, SnapshotType.DESTPOS);
            packet.Write(movableEntity.Moves.DestinationPosition.X);
            packet.Write(movableEntity.Moves.DestinationPosition.Y);
            packet.Write(movableEntity.Moves.DestinationPosition.Z);
            packet.Write<byte>(1);

            SendToVisible(packet, movableEntity);
        }

        /// <inheritdoc />
        public void SendMoverPosition(IWorldEntity entity)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOS);
            packet.Write(entity.Object.Position.X);
            packet.Write(entity.Object.Position.Y);
            packet.Write(entity.Object.Position.Z);

            SendToVisible(packet, entity, sendToPlayer: true);
        }

        /// <inheritdoc />
        public void SendMoverPositionAngle(IWorldEntity entity, bool sendOwnPlayer = true)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOSANGLE);
            packet.Write(entity.Object.Position.X);
            packet.Write(entity.Object.Position.Y);
            packet.Write(entity.Object.Position.Z);
            packet.Write(entity.Object.Angle);

            SendToVisible(packet, entity, sendToPlayer: sendOwnPlayer);
        }

        /// <inheritdoc />
        public void SendStateMode(IWorldEntity entity, StateModeBaseMotion flags, InventoryItem item = null)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.STATEMODE);
            packet.Write((int)entity.Object.StateMode);
            packet.Write((byte)flags);

            if (flags == StateModeBaseMotion.BASEMOTION_ON && item != null)
                packet.Write(item.Id);

            SendToVisible(packet, entity, sendToPlayer: true);
        }

        /// <inheritdoc />
        public void SendFollowTarget(IWorldEntity entity, IWorldEntity targetEntity, float distance)
        {
            if (entity is IPlayerEntity playerEntity)
            {
                if (playerEntity.PlayerData.Mode.HasFlag(ModeType.TRANSPARENT_MODE)) 
                {
                    return;
                }
            }

            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERSETDESTOBJ);
            packet.Write(targetEntity.Id);
            packet.Write(distance);

            SendToVisible(packet, entity);
        }

        /// <inheritdoc />
        public void SendUpdateAttributes(IWorldEntity entity, DefineAttributes attribute, int newValue)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOINTPARAM);
            packet.Write((int)attribute);
            packet.Write(newValue);

            SendToVisible(packet, entity, true);
        }

        /// <inheritdoc />
        public void SendMotion(IWorldEntity entity, ObjectMessageType objectMessage)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.MOTION);
            packet.Write((int)objectMessage);

            SendToVisible(packet, entity, sendToPlayer: true);
        }

        /// <inheritdoc />
        public void SendSpeedFactor(IWorldEntity entity, float speedFactor)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.SET_SPEED_FACTOR);
            packet.Write(speedFactor);

            SendToVisible(packet, entity);
        }
    }
}

using System;
using System.Reflection;
using Ether.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.Core.Structures;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Packets;

namespace Rhisis.World.Handlers
{
    public static class MovementHandler
    {
        [PacketHandler(PacketType.PLAYERMOVED)]
        public static void OnMovement(WorldClient client, INetPacketStream packet)
        {
            var onPlayerMovedPacket = new OnPlayerMovedPacket(packet);

            if (client.Player.Health.IsDead) // || player is flying
                return;

            Vector3 vDistance = client.Player.Object.Position - onPlayerMovedPacket.BeginPosition;
            if (vDistance.SquaredLength > 1000000.0f)
                return;

            int delay = 0; //should be this: int delay = (int)( (float)g_TickCount.GetOffset( onPlayerMovedPacket.TickCount ) / 66.6667f );

            // delay >= MAX_CORR_SIZE_45 // MAX_CORR_SIZE_45 = 5
            if (delay >= 5) // should be this in the end: delay >= 5 || (client.Player.IsStateFlag(OBJSTAF_FLY) || onPlayerMovedPacket.StateFlag & OBJSTAF_FLY))
            {
                // https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/_Common/MoverParam.cpp#L3476
                // ActionForceSet(onPlayerMovedPacket.v, onPlayerMovedPacket.vd, onPlayerMovedPacket.f, onPlayerMovedPacket.State, onPlayerMovedPacket.StateFlag, onPlayerMovedPacket.Motion, onPlayerMovedPacket.MotionEx, onPlayerMovedPacket.Loop, onPlayerMovedPacket.MotionOption);
            }
            else
            {
                client.Player.MovableComponent.Speed = 0;

                int remnant = 5 - delay; // int nRemnant	= (int)( MAX_CORR_SIZE_45 - delay );
                Vector3 d = onPlayerMovedPacket.BeginPosition - client.Player.Object.Position;
                float length = d.Length;
                float speed = length / remnant / 4 + 0.020f;

                client.Player.MovableComponent.Speed = /* client.Player.IsStateFlag(OBJSTAF_WALK) ? speed * 4.0f : speed;*/ speed * 4.0f;
                client.Player.MovableComponent.BeginPosition = onPlayerMovedPacket.BeginPosition;
                client.Player.MovableComponent.DestinationPosition = onPlayerMovedPacket.DestinationPosition;
                client.Player.Object.Angle = onPlayerMovedPacket.Angle;

                // TODO: We need to set the motion stuff here when it's implemented.
                /*
                pUser->m_CorrAction.dwState	= dwState;
                pUser->m_CorrAction.dwStateFlag	= dwStateFlag;
                pUser->m_CorrAction.dwMotion	= dwMotion;
                pUser->m_CorrAction.nMotionEx	= nMotionEx;
                pUser->m_CorrAction.nLoop	= nLoop;
                pUser->m_CorrAction.dwMotionOption	= dwMotionOption;
                pUser->m_CorrAction.fValid	= TRUE;
                 */

                client.Player.MovableComponent.DestinationPosition = onPlayerMovedPacket.BeginPosition;
            }
            WorldPacketFactory.SendMoverMoved(client.Player, onPlayerMovedPacket.BeginPosition, onPlayerMovedPacket.DestinationPosition, onPlayerMovedPacket.Angle, onPlayerMovedPacket.State, onPlayerMovedPacket.StateFlag, onPlayerMovedPacket.Motion, onPlayerMovedPacket.MotionEx, onPlayerMovedPacket.Loop, onPlayerMovedPacket.MotionOption, onPlayerMovedPacket.TickCount);
        }

        public static void OnSnapshotSetDestPosition(WorldClient client, INetPacketStream packet)
        {
            var setDestPositionPacket = new SetDestPositionPacket(packet);

            client.Player.MovableComponent.DestinationPosition = new Vector3(setDestPositionPacket.X, setDestPositionPacket.Y, setDestPositionPacket.Z);
            client.Player.Object.Angle = Vector3.AngleBetween(client.Player.Object.Position, client.Player.MovableComponent.DestinationPosition);
            client.Player.Follow.Target = null;

            WorldPacketFactory.SendDestinationPosition(client.Player);
        }
    }
}

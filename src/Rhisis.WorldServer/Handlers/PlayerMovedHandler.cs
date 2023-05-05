
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Client;
using Rhisis.Game.Protocol.Packets.World.Server;
using Rhisis.Protocol;
using Rhisis.Protocol.Handlers;
using System;

namespace Rhisis.WorldServer.Handlers;

[PacketHandler(PacketType.PLAYERMOVED)]
internal sealed class PlayerMovedHandler : WorldPacketHandler
{
    public void Execute(MoverMovedPacket packet)
    {
        if (Player.IsDead)
        {
            throw new InvalidOperationException("Player is dead.");
        }

        // TODO: this handler isn't really correct.
        // We need to review this in order to correct movements.

        Player.Unfollow();
        Player.DestinationPosition.Reset();
        Player.Position.Copy(packet.BeginPosition + packet.DestinationPosition);
        Player.RotationAngle = packet.Angle;
        Player.ObjectState = (ObjectState)packet.State;
        Player.ObjectStateFlags = (StateFlags)packet.StateFlag;

        using var snapshot = new MoverMovedSnapshot(Player, 
            packet.BeginPosition, packet.DestinationPosition, Player.RotationAngle, 
            (int)Player.ObjectState, (int)Player.ObjectStateFlags, 
            (int)packet.Motion, packet.MotionEx, packet.Loop, (int)packet.MotionOption, packet.TickCount);

        Player.SendToVisible(snapshot);
    }
}
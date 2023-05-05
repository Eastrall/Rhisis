
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Client;
using Rhisis.Game.Protocol.Packets.World.Server;
using Rhisis.Protocol;
using Rhisis.Protocol.Handlers;
using System;

namespace Rhisis.WorldServer.Handlers;

[PacketHandler(PacketType.PLAYERBEHAVIOR)]
internal sealed class PlayerBehaviorHandler : WorldPacketHandler
{
    public void Execute(PlayerMovedPacket packet)
    {
        if (Player.IsDead)
        {
            throw new InvalidOperationException("Player is dead.");
        }

        // TODO: this handler isn't really correct.
        // We need to review this in order to correct movements.

        Player.Unfollow();
        // Player.Battle.ClearTarget();
        Player.DestinationPosition.Reset();
        Player.Position.Copy(packet.BeginPosition + packet.DestinationPosition);
        Player.RotationAngle = packet.Angle;
        Player.ObjectState = (ObjectState)packet.State;
        Player.ObjectStateFlags = (StateFlags)packet.StateFlag;

        using var snapshot = new MoverBehaviorSnapshot(Player,
            packet.BeginPosition, packet.DestinationPosition, Player.RotationAngle,
            (int)Player.ObjectState, (int)Player.ObjectStateFlags,
            packet.Motion, packet.MotionEx, packet.Loop, packet.MotionOption, packet.TickCount);

        Player.SendToVisible(snapshot);
    }
}
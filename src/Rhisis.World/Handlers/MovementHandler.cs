using Ether.Network.Packets;
using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Packets;
using Rhisis.World.Systems.SpecialEffect;
using Rhisis.World.Systems.SpecialEffect.EventArgs;
using System;

namespace Rhisis.World.Handlers
{
    public static class MovementHandler
    {
        public static void OnSnapshotSetDestPosition(WorldClient client, INetPacketStream packet)
        {
            var setDestPositionPacket = new SetDestPositionPacket(packet);

            // Cancel current item usage action and SFX
            SystemManager.Instance.Execute<SpecialEffectSystem>(client.Player, new SpecialEffectBaseMotionEventArgs(StateModeBaseMotion.BASEMOTION_OFF));
            client.Player.Delayer.CancelAction(client.Player.Inventory.ItemInUseActionId);
            client.Player.Inventory.ItemInUseActionId = Guid.Empty;

            client.Player.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;
            client.Player.Moves.DestinationPosition = new Vector3(setDestPositionPacket.X, setDestPositionPacket.Y, setDestPositionPacket.Z);
            client.Player.Object.Angle = Vector3.AngleBetween(client.Player.Object.Position, client.Player.Moves.DestinationPosition);
            client.Player.Follow.Target = null;

            WorldPacketFactory.SendDestinationPosition(client.Player);
        }
    }
}

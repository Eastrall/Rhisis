using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Packets;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class MovementHandler
    {
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="MovementHandler"/> instance.
        /// </summary>
        /// <param name="moverPacketFactory">Mover packet factory.</param>
        public MovementHandler(IMoverPacketFactory moverPacketFactory)
        {
            this._moverPacketFactory = moverPacketFactory;
        }

        /// <summary>
        /// Handles the destination position snapshot.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(SnapshotType.DESTPOS)]
        public void OnSnapshotSetDestPosition(IWorldClient client, SetDestPositionPacket packet)
        {
            // Cancel current item usage action and SFX
            //SystemManager.Instance.Execute<SpecialEffectSystem>(client.Player, new SpecialEffectBaseMotionEventArgs(StateModeBaseMotion.BASEMOTION_OFF));

            client.Player.Delayer.CancelAction(client.Player.Inventory.ItemInUseActionId);
            client.Player.Inventory.ItemInUseActionId = Guid.Empty;

            client.Player.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;
            client.Player.Moves.DestinationPosition = new Vector3(packet.X, packet.Y, packet.Z);
            client.Player.Object.Angle = Vector3.AngleBetween(client.Player.Object.Position, client.Player.Moves.DestinationPosition);
            client.Player.Follow.Target = null;

            this._moverPacketFactory.SendDestinationPosition(client.Player);
        }
    }
}

using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.Dialog;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class NpcDialogHandler
    {
        private readonly IDialogSystem _dialogSystem;

        /// <summary>
        /// Creates a new <see cref="NpcDialogHandler"/> instance.
        /// </summary>
        /// <param name="dialogSystem">Dialog system.</param>
        public NpcDialogHandler(IDialogSystem dialogSystem)
        {
            this._dialogSystem = dialogSystem;
        }

        /// <summary>
        /// Opens a dialog script.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming <see cref="DialogPacket"/>.</param>
        [HandlerAction(PacketType.SCRIPTDLG)]
        public void OnDialogScript(IWorldClient client, DialogPacket packet)
        {
            if (packet.ObjectId <= 0)
            {
                throw new ArgumentException("Invalid object id.");
            }

            this._dialogSystem.OpenNpcDialog(client.Player, packet.ObjectId, packet.Key);
        }
    }
}
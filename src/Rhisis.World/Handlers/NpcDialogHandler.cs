using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.NpcDialog;

namespace Rhisis.World.Handlers
{
    public static class NpcDialogHandler
    {
        [PacketHandler(PacketType.SCRIPTDLG)]
        public static void OnDialogScript(WorldClient client, INetPacketStream packet)
        {
            var dialogPacket = new DialogPacket(packet);
            var dialogEvent = new NpcDialogOpenEventArgs(dialogPacket.ObjectId, dialogPacket.Key);

            SystemManager.Instance.Execute<NpcDialogSystem>(client.Player, dialogEvent);
        }
    }
}
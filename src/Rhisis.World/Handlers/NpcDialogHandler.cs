using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.NpcDialog;

namespace Rhisis.World.Handlers
{
    public static class NpcDialogHandler
    {
        [PacketHandler(PacketType.SCRIPTDLG)]
        public static void OnDialogScript(WorldClient client, INetPacketStream packet)
        {
            var objectId = packet.Read<int>();
            var key = packet.Read<string>();
            var param = packet.Read<int>();
            var questId = packet.Read<int>();
            var moverId = packet.Read<int>();
            var playerId = packet.Read<int>();
            var dialogEvent = new NpcDialogOpenEventArgs(objectId, key);

            client.Player.NotifySystem<NpcDialogSystem>(dialogEvent);
        }
    }
}
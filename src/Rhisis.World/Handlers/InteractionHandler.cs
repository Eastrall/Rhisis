using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Interaction;
using Rhisis.World.Systems.Interaction.EventArgs;

namespace Rhisis.World.Handlers
{
    public static class InteractionHandler
    {
        [PacketHandler(PacketType.SETTARGET)]
        public static void OnSetTarget(WorldClient client, INetPacketStream packet)
        {
            var setTargetPacket = new SetTargetPacket(packet);
            var setTargetArgs = new SetTargetEventArgs(setTargetPacket.TargetId, setTargetPacket.Clear);

            SystemManager.Instance.Execute<InteractionSystem>(client.Player, setTargetArgs);
        }
    }
}
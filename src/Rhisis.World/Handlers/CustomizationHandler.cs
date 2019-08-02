using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Customization;
using Rhisis.World.Systems.Customization.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class CustomizationHandler
    {
        [PacketHandler(PacketType.CHANGEFACE)]
        public static void OnChangeFace(WorldClient client, INetPacketStream packet)
        {
            var changeFacePacket = new ChangeFacePacket(packet);
            var changeFaceEventArgs = new ChangeFaceEventArgs(changeFacePacket.ObjectId, changeFacePacket.FaceNumber, changeFacePacket.UseCoupon);

            SystemManager.Instance.Execute<CustomizationSystem>(client.Player, changeFaceEventArgs);
        }

        [PacketHandler(PacketType.SET_HAIR)]
        public static void OnSetHair(WorldClient client, INetPacketStream packet)
        {
            var setHairPacket = new SetHairPacket(packet);
            var setHairEventArgs = new SetHairEventArgs(setHairPacket.HairId, setHairPacket.R, setHairPacket.G, setHairPacket.B, setHairPacket.UseCoupon);

            SystemManager.Instance.Execute<CustomizationSystem>(client.Player, setHairEventArgs);
        }
    }
}

using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.Customization;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    /// <summary>
    /// Handles requests related to the player customization system.
    /// </summary>
    [Handler]
    public class CustomizationHandler
    {
        private readonly ICustomizationSystem _customizationSystem;

        /// <summary>
        /// Creates a new <see cref="CustomizationHandler"/> instance.
        /// </summary>
        /// <param name="customizationSystem">Customization system.</param>
        public CustomizationHandler(ICustomizationSystem customizationSystem)
        {
            this._customizationSystem = customizationSystem;
        }

        /// <summary>
        /// Changes the face of a player.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.CHANGEFACE)]
        public void OnChangeFace(IWorldClient client, ChangeFacePacket packet)
        {
            this._customizationSystem.ChangeFace(client.Player, packet.ObjectId, packet.FaceNumber, packet.UseCoupon);
        }

        /// <summary>
        /// Changes the hair and color of a player.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.SET_HAIR)]
        public void OnSetHair(IWorldClient client, SetHairPacket packet)
        {
            this._customizationSystem.ChangeHair(client.Player, packet.HairId, packet.R, packet.G, packet.B, packet.UseCoupon);
        }
    }
}

using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface ICustomizationPacketFactory
    {
        /// <summary>
        /// Sends a packet that changes the player's face.
        /// </summary>
        /// <param name="entity">Player.</param>
        /// <param name="faceId">New player face id.</param>
        void SendChangeFace(IPlayerEntity entity, uint faceId);

        /// <summary>
        /// Sends a packet that changes the player's hair and hair color.
        /// </summary>
        /// <param name="entity">Player.</param>
        /// <param name="hairId">New player hair id.</param>
        /// <param name="r">Red color.</param>
        /// <param name="g">Green color.</param>
        /// <param name="b">Blue color.</param>
        void SendSetHair(IPlayerEntity entity, byte hairId, byte r, byte g, byte b);
    }
}

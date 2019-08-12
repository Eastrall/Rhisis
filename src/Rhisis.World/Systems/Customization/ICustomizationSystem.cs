using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Customization
{
    public interface ICustomizationSystem
    {
        /// <summary>
        /// Changes the face of a player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="objectId">Player object id.</param>
        /// <param name="faceId">New face id.</param>
        /// <param name="useCoupon">Use coupon.</param>
        void ChangeFace(IPlayerEntity player, uint objectId, uint faceId, bool useCoupon);

        /// <summary>
        /// Changes the hair style and color of a player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="hairId">New hair id.</param>
        /// <param name="r">Red color.</param>
        /// <param name="g">Green color.</param>
        /// <param name="b">Blue color.</param>
        /// <param name="useCoupon">Use coupon.</param>
        void ChangeHair(IPlayerEntity player, byte hairId, byte r, byte g, byte b, bool useCoupon);
    }
}

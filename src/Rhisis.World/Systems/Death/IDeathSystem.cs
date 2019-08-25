using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Death
{
    public interface IDeathSystem
    {
        /// <summary>
        /// Resurects the player to the nearst revival point.
        /// </summary>
        /// <param name="player">Current player.</param>
        void ResurectLodelight(IPlayerEntity player);
    }
}

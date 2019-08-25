using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    public interface IRegionTriggerSystem
    {
        /// <summary>
        /// Checks if the current player's position intersects a wrapzone.
        /// If it intersects then we teleport the player to the destination map.
        /// </summary>
        /// <param name="player">Current player.</param>
        void CheckWrapzones(IPlayerEntity player);
    }
}

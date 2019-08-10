using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.PlayerData
{
    public interface IPlayerDataSystem
    {
        /// <summary>
        /// Saves a player.
        /// </summary>
        /// <param name="player">Player entity to save.</param>
        void SavePlayer(IPlayerEntity player);
    }
}

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

        /// <summary>
        /// Increases the player's gold amount.
        /// </summary>
        /// <param name="player">Player entity to increase gold.</param>
        /// <param name="goldAmount">Gold amount to increase.</param>
        /// <returns>Returns true if the gold has been increased; false otherwhise.</returns>
        bool IncreaseGold(IPlayerEntity player, int goldAmount);

        /// <summary>
        /// Decreases the player's gold amount.
        /// </summary>
        /// <param name="player">Player entity to decrease gold.</param>
        /// <param name="goldAmount">Gold amount to decrease.</param>
        /// <returns>Returns true if the gold has been decreased; false otherwhise.</returns>
        bool DecreaseGold(IPlayerEntity player, int goldAmount);
    }
}

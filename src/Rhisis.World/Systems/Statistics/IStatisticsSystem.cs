using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Statistics
{
    public interface IStatisticsSystem
    {
        /// <summary>
        /// Updates the player's statistics.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="strength">Strength.</param>
        /// <param name="stamina">Stamina.</param>
        /// <param name="dexterity">Dexterity.</param>
        /// <param name="intelligence">Intelligence.</param>
        void UpdateStatistics(IPlayerEntity player, ushort strength, ushort stamina, ushort dexterity, ushort intelligence);
    }
}

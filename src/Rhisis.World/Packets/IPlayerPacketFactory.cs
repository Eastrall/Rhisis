using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IPlayerPacketFactory
    {
        /// <summary>
        /// Sends a packet that teleports the player to another position.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerTeleport(IPlayerEntity player);

        /// <summary>
        /// Sends a packet that replaces the current player position.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerReplace(IPlayerEntity player);

        /// <summary>
        /// Sends a packet that updates the player's statistics.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerUpdateState(IPlayerEntity player);

        /// <summary>
        /// Sends a packet that updates the amount of player's statistics points.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerStatsPoints(IPlayerEntity player);

        /// <summary>
        /// Sends a packet that updates the player's level.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="level">Player level.</param>
        void SendPlayerSetLevel(IPlayerEntity player, int level);

        /// <summary>
        /// Sends a packet that updates the player's experience.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerExperience(IPlayerEntity player);

        /// <summary>
        /// Sends a packet that sends the revival interface to the player.
        /// </summary>
        /// <param name="player">Current player.</param>
        void SendPlayerRevival(IPlayerEntity player);
    }
}

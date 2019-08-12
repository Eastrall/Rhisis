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
    }
}

using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IChatPacketFactory
    {
        /// <summary>
        /// Sends a chat packet to all players around the current player.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="message">Message</param>
        void SendChat(IPlayerEntity player, string message);

        /// <summary>
        /// Sends a chat message to a given player only.
        /// </summary>
        /// <remarks>
        /// Used in NPC oral text.
        /// </remarks>
        /// <param name="fromEntity">Entity.</param>
        /// <param name="toPlayer">Destination player.</param>
        /// <param name="message">Message.</param>
        void SendChatTo(IWorldEntity fromEntity, IPlayerEntity toPlayer, string message);
    }
}

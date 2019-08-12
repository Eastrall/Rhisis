using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Chat
{
    public interface IChatSystem
    {
        /// <summary>
        /// Sends a chat message or executes a chat command.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="chatMessage">Chat message.</param>
        void Chat(IPlayerEntity player, string chatMessage);
    }
}

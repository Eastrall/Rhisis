using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Chat
{
    /// <summary>
    /// Provides a mechanism to create chat commands.
    /// </summary>
    public interface IChatCommand
    {
        /// <summary>
        /// Executes the chat command.
        /// </summary>
        /// <param name="parameters">Chat commands parameters.</param>
        void Execute(IPlayerEntity player, object[] parameters);
    }
}
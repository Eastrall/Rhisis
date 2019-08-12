using Rhisis.Core.Common;

namespace Rhisis.World.Game.Chat
{
    public interface IChatCommandManager
    {
        /// <summary>
        /// Loads all chat commands.
        /// </summary>
        void Load();

        /// <summary>
        /// Gets a chat command by its command name.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="authority">Executor authority.</param>
        IChatCommand GetChatCommand(string command, AuthorityType authority);
    }
}
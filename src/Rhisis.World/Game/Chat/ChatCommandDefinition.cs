using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;

namespace Rhisis.World.Game.Chat
{
    /// <summary>
    /// Defines a chat command.
    /// </summary>
    internal sealed class ChatCommandDefinition
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the command factory.
        /// </summary>
        public ObjectFactory ChatCommandFactory { get; }

        /// <summary>
        /// Gets the command minimum authority.
        /// </summary>
        public AuthorityType Authority { get; }

        /// <summary>
        /// Creates a new <see cref="ChatCommandDefinition"/> instance.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <param name="factory">Command factory.</param>
        /// <param name="minimumAuthority">Command minimum authority to execute.</param>
        public ChatCommandDefinition(string command, ObjectFactory factory, AuthorityType minimumAuthority)
        {
            this.Command = command;
            this.ChatCommandFactory = factory;
            this.Authority = minimumAuthority;
        }
    }
}
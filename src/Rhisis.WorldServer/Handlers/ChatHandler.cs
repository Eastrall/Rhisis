using Rhisis.Game;
using Rhisis.Game.Abstractions.Entities;
using Rhisis.Game.Abstractions.Features.Chat;
using Rhisis.Network;
using Rhisis.Network.Packets.World;
using Sylver.HandlerInvoker.Attributes;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.WorldServer.Handlers
{
    /// <summary>
    /// Handles every packets related to the chat system.
    /// </summary>
    [Handler]
    public sealed class ChatHandler
    {
        private readonly IChatCommandManager _chatCommandManager;

        /// <summary>
        /// Creates a new <see cref="ChatHandler"/> instance.
        /// </summary>
        /// <param name="chatCommandManager">Chat command manager.</param>
        public ChatHandler(IChatCommandManager chatCommandManager)
        {
            _chatCommandManager = chatCommandManager;
        }

        /// <summary>
        /// Handles a chat request.
        /// </summary>
        /// <param name="serverClient"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.CHAT)]
        public void OnChat(IPlayer player, ChatPacket packet)
        {
            if (!string.IsNullOrEmpty(packet.Message))
            {
                if (packet.Message.StartsWith(GameConstants.ChatCommandPrefix))
                {
                    string commandName = GetCommandName(packet.Message);
                    IChatCommand chatCommand = _chatCommandManager.GetChatCommand(commandName, player.Authority);
                    string[] commandParameters = GetCommandParameters(packet.Message, commandName, chatCommand.ParsingType);

                    if (chatCommand == null)
                    {
                        throw new ArgumentException($"Cannot find chat command: '/{commandName}'", nameof(chatCommand));
                    }

                    chatCommand.Execute(player, commandParameters);
                }
                else
                {
                    player.Chat.Speak(packet.Message);
                }
            }
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        /// <param name="commandInput">Command input.</param>
        /// <returns>Command name.</returns>
        private string GetCommandName(string commandInput)
        {
            return commandInput.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        /// <param name="commandInput">Command input text.</param>
        /// <param name="commandName">Command name</param>
        /// <param name="parsingType">Command parsing type.</param>
        /// <returns></returns>
        private string[] GetCommandParameters(string commandInput, string commandName, ChatParameterParsingType parsingType)
        {
            if (parsingType == ChatParameterParsingType.Default)
            {
                string commandParameters = commandInput.Remove(0, commandName.Length);

                return Regex.Matches(commandParameters, @"[\""].+?[\""]|[^ ]+").Select(m => m.Value.Trim('"')).ToArray();
            }
            else
            {
                return new[] { commandInput.Replace(commandName, "").Trim() };
            }
        }
    }
}

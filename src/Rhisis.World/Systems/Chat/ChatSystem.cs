using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Chat;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.World.Systems.Chat
{
    [Injectable]
    public sealed class ChatSystem : IChatSystem
    {
        public const string ChatCommandPrefix = "/";
        private readonly ILogger<ChatSystem> _logger;
        private readonly IChatCommandManager _chatCommandManager;
        private readonly IChatPacketFactory _chatPacketFactory;

        /// <summary>
        /// Creates a new <see cref="ChatSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="chatCommandManager">Chat command manager.</param>
        /// <param name="chatPacketFactory">Chat packet factory.</param>
        public ChatSystem(ILogger<ChatSystem> logger, IChatCommandManager chatCommandManager, IChatPacketFactory chatPacketFactory)
        {
            this._logger = logger;
            this._chatCommandManager = chatCommandManager;
            this._chatPacketFactory = chatPacketFactory;
        }

        /// <inheritdoc />
        public void Chat(IPlayerEntity player, string chatMessage)
        {
            if (string.IsNullOrEmpty(chatMessage))
            {
                throw new ArgumentNullException(nameof(chatMessage), $"Cannot send an empty message for player {player.Object.Name}.");
            }

            if (chatMessage.StartsWith(ChatCommandPrefix))
            {
                (string, string[]) commandInfo = this.GetCommandParameters(chatMessage);
                IChatCommand chatCommand = this._chatCommandManager.GetChatCommand(commandInfo.Item1, player.PlayerData.Authority);

                if (chatCommand == null)
                {
                    throw new ArgumentNullException(nameof(chatCommand), $"Cannot find chat command: ");
                }

                chatCommand.Execute(player, commandInfo.Item2);
            }
            else
            {
                this._chatPacketFactory.SendChat(player, chatMessage);
            }
        }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        /// <param name="command">Command line</param>
        /// <param name="commandName">Command name</param>
        /// <returns></returns>
        private (string, string[]) GetCommandParameters(string command)
        {
            string commandName = command.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (string.IsNullOrEmpty(commandName))
                return (command, Enumerable.Empty<string>().ToArray());

            string commandParameters = command.Remove(0, commandName.Length);

            return (commandName, Regex.Matches(commandParameters, @"[\""].+?[\""]|[^ ]+").Select(m => m.Value.Trim('"')).ToArray());
        }
    }
}

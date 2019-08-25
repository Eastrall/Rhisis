using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhisis.World.Game.Chat
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class ChatCommandManager : IChatCommandManager
    {
        private readonly ILogger<ChatCommandManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, ChatCommandDefinition> _chatCommands;

        /// <summary>
        /// Creates a new <see cref="ChatCommandManager"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public ChatCommandManager(ILogger<ChatCommandManager> logger, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._chatCommands = new ConcurrentDictionary<string, ChatCommandDefinition>();
        }

        /// <inheritdoc />
        public void Load()
        {
            this._logger.LogInformation($"Loading chat commands...");

            IEnumerable<TypeInfo> chatCommandTypes = ReflectionHelper.GetClassesAssignableFrom<IChatCommand>();

            foreach (TypeInfo chatCommandType in chatCommandTypes)
            {
                IEnumerable<ChatCommandAttribute> attributes = chatCommandType.GetCustomAttributes<ChatCommandAttribute>();

                if (attributes.Any())
                {
                    ObjectFactory chatCommandFactory = ActivatorUtilities.CreateFactory(chatCommandType, Type.EmptyTypes);

                    foreach (ChatCommandAttribute chatAttribute in attributes)
                    {
                        var chatCommandDefinition = new ChatCommandDefinition(chatAttribute.Command, chatCommandFactory, chatAttribute.MinimumAuthorization);

                        if (!this._chatCommands.TryAdd(chatAttribute.Command, chatCommandDefinition))
                        {
                            throw new InvalidOperationException($"Duplicate chat command: '{chatAttribute.Command}' already exists.");
                        }
                    }
                }
            }

            this._logger.LogInformation($"-> {this._chatCommands.Count} chat commands loaded.");
        }

        /// <inheritdoc />
        public IChatCommand GetChatCommand(string command, AuthorityType authority)
        {
            if (this._chatCommands.TryGetValue(command, out ChatCommandDefinition chatCommandDefinition))
            {
                if (authority < chatCommandDefinition.Authority)
                {
                    throw new InvalidOperationException($"Player doesn't have enough authority to execute this command.");
                }

                return chatCommandDefinition.ChatCommandFactory(this._serviceProvider, null) as IChatCommand;
            }
            else
            {
                throw new KeyNotFoundException($"Cannot find chat command: '{command}'.");
            }
        }
    }
}
using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.Helpers;
using Rhisis.World.Game.Chat;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rhisis.World.Systems.Chat
{
    [System(SystemType.Notifiable)]
    public class ChatSystem : ISystem
    {
        private sealed class ChatCommandDefinition
        {
            /// <summary>
            /// The method to invoke.
            /// </summary>
            public Action<IPlayerEntity, string[]> Method { get; }

            /// <summary>
            /// The Minimum Authorization for this command.
            /// </summary>
            public AuthorityType MinAuthorization { get; }

            /// <summary>
            /// Creates a new <see cref="ChatCommandDefinition"/> instance.
            /// </summary>
            /// <param name="method"></param>
            /// <param name="minAuthorization"></param>
            public ChatCommandDefinition(Action<IPlayerEntity, string[]> method, AuthorityType minAuthorization)
            {
                Method = method;
                MinAuthorization = minAuthorization;
            }
        }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IDictionary<string, ChatCommandDefinition> ChatCommands = new Dictionary<string, ChatCommandDefinition>();
        private const string MsgUnableExecuteCmd = "Unable to execute chat command '{0}' for player '{1}'. Reason: {2}.";

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!(e is ChatEventArgs chatEvent) || !(entity is IPlayerEntity player))
                return;
            if (!chatEvent.GetCheckArguments())
                return;

            if (chatEvent.Message.StartsWith("/") || chatEvent.Message.StartsWith("."))
            {
                string commandName = chatEvent.Message.Split(' ').FirstOrDefault();
                string[] commandParameters = GetCommandParameters(chatEvent.Message, commandName);

                if (ChatCommands.TryGetValue(commandName, out var command))
                {
                    if (player.PlayerData.Authority < command.MinAuthorization)
                        Logger.Warn(MsgUnableExecuteCmd, commandName, player.Object.Name, "player has no privileges");
                    else
                    {
                        command.Method.Invoke(player, commandParameters);
                        Logger.Info("Command '{0}' executed for player '{1}'.", commandName, player.Object.Name);
                    }
                }
                else
                    Logger.Warn(MsgUnableExecuteCmd, commandName, player.Object.Name, "unknown command");
            }
            else
            {
                WorldPacketFactory.SendChat(player, chatEvent.Message);
            }
        }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        /// <param name="command">Command line</param>
        /// <param name="commandName">Command name</param>
        /// <returns></returns>
        private static string[] GetCommandParameters(string command, string commandName)
        {
            string commandParameters = command.Remove(0, commandName.Length);

            return Regex.Matches(commandParameters, @"[\""].+?[\""]|[^ ]+").Select(m => m.Value.Trim('"')).ToArray();
        }

        /// <summary>
        /// Initialize the system.
        /// </summary>
        public static void Initialize()
        {
            IEnumerable<MethodInfo> chatCommandsMethods = ReflectionHelper.GetMethodsWithAttributes<ChatCommandAttribute>();

            foreach (MethodInfo chatMethod in chatCommandsMethods)
            {
                var action = chatMethod.CreateDelegate(typeof(Action<IPlayerEntity, string[]>)) as Action<IPlayerEntity, string[]>;
                IEnumerable<ChatCommandAttribute> chatComandAttributes = chatMethod.GetCustomAttributes<ChatCommandAttribute>();

                foreach (ChatCommandAttribute attribute in chatComandAttributes)
                {
                    if (!ChatCommands.ContainsKey(attribute.Command))
                        ChatCommands.Add(attribute.Command, new ChatCommandDefinition(action, attribute.MinAuthorization));
                }
            }
        }
    }
}

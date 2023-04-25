using Rhisis.Game.Chat;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Client;
using Rhisis.Protocol;
using Rhisis.Protocol.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.WorldServer.Handlers;

[PacketHandler(PacketType.CHAT)]
internal sealed class ChatHandler : WorldPacketHandler
{
    private static readonly List<string> _emotes = new()
    {
      "/laugh",
      "/sad",
      "/kiss",
      "/surprise",
      "/blush",
      "/anger",
      "/sigh",
      "/wink",
      "/ache",
      "/hunger",
      "/yummy",
      "/sneer",
      "/sparkle",
      "/ridicule",
      "/sleepy",
      "/rich",
      "/glare",
      "/sweat",
      "/cat face",
      "/tongue",
      "/mad",
      "/aha",
      "/embarrassed",
      "/help",
      "/crazy",
      "/oh!",
      "/confused",
      "/ouch",
      "/love",
    };

    public void Execute(ChatPacket packet)
    {
        if (!string.IsNullOrEmpty(packet.Message))
        {
            if (packet.Message.StartsWith("/"))
            {
                if (_emotes.Contains(packet.Message))
                {
                    Player.Speak(packet.Message);
                }
                else
                {
                    string commandName = packet.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    IChatCommand chatCommand = ChatCommandManager.Current.Get(commandName, Player.Authority) ??
                     throw new ArgumentException($"Cannot find chat command: '/{commandName}'", nameof(chatCommand));

                    string[] commandParameters = GetCommandParameters(packet.Message, commandName, chatCommand.ParsingType);
                }

            }
            else
            {
                Player.Speak(packet.Message);
            }
        }
    }

    /// <summary>
    /// Gets the command parameters.
    /// </summary>
    /// <param name="commandInput">Command input text.</param>
    /// <param name="commandName">Command name</param>
    /// <param name="parsingType">Command parsing type.</param>
    /// <returns></returns>
    private static string[] GetCommandParameters(string commandInput, string commandName, ChatParameterParsingType parsingType)
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


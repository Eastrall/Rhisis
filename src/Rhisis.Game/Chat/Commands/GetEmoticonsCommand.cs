using Microsoft.Extensions.Logging;
using Rhisis.Game.Common;
using Rhisis.Game.Entities;

namespace Rhisis.Game.Chat.Commands;

[ChatCommand("/laugh", AuthorityType.Player)]
[ChatCommand("/sad", AuthorityType.Player)]
[ChatCommand("/kiss", AuthorityType.Player)]
[ChatCommand("/surprise", AuthorityType.Player)]
[ChatCommand("/blush", AuthorityType.Player)]
[ChatCommand("/anger", AuthorityType.Player)]
[ChatCommand("/sigh", AuthorityType.Player)]
[ChatCommand("/wink", AuthorityType.Player)]
[ChatCommand("/ache", AuthorityType.Player)]
[ChatCommand("/hunger", AuthorityType.Player)]
[ChatCommand("/yummy", AuthorityType.Player)]
[ChatCommand("/sneer", AuthorityType.Player)]
[ChatCommand("/sparkle", AuthorityType.Player)]
[ChatCommand("/ridicule", AuthorityType.Player)]
[ChatCommand("/sleepy", AuthorityType.Player)]
[ChatCommand("/rich", AuthorityType.Player)]
[ChatCommand("/glare", AuthorityType.Player)]
[ChatCommand("/sweat", AuthorityType.Player)]
[ChatCommand("/cat", AuthorityType.Player)]
[ChatCommand("/tongue", AuthorityType.Player)]
[ChatCommand("/mad", AuthorityType.Player)]
[ChatCommand("/aha", AuthorityType.Player)]
[ChatCommand("/embarrassed", AuthorityType.Player)]
[ChatCommand("/help", AuthorityType.Player)]
[ChatCommand("/crazy", AuthorityType.Player)]
[ChatCommand("/oh!", AuthorityType.Player)]
[ChatCommand("/confused", AuthorityType.Player)]
[ChatCommand("/ouch", AuthorityType.Player)]
[ChatCommand("/love", AuthorityType.Player)]
internal sealed class GetEmoticonsCommand : IChatCommand 
{
    private readonly ILogger<GetEmoticonsCommand> _logger;

    /// <summary>
    /// Creates a new <see cref="GetEmoticonsCommand"/> instance.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public GetEmoticonsCommand(ILogger<GetEmoticonsCommand> logger)
    {
        _logger = logger;
    }

    public void Execute(Player player, object[] parameters) { }
}
 

using Microsoft.Extensions.Logging;
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Protocol.Snapshots;

namespace Rhisis.Game.Chat.Commands;

[ChatCommand("/onekill", AuthorityType.GameMaster)]
[ChatCommand("/ok", AuthorityType.GameMaster)]
internal sealed class OneKillCommand : IChatCommand
{
    private readonly ILogger<OneKillCommand> _logger;

    /// <summary>
    /// Creates a new <see cref="OneKillChatCommand"/> instance.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public OneKillCommand(ILogger<OneKillCommand> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void Execute(Player player, object[] parameters)
    {
        if (!player.Mode.HasFlag(ModeType.ONEKILL_MODE))
        {
            player.Mode |= ModeType.ONEKILL_MODE;

            using (var snapshot = new ModifyModeSnapshot(player, player.Mode))
            {
                player.Send(snapshot);
                player.SendToVisible(snapshot);
            }

            _logger.LogTrace($"Player '{player.Name}' is now in OneKill mode.");
        }
        else
        {
            _logger.LogTrace($"Player '{player.Name}' is already in OneKill mode.");
        }
    }
}
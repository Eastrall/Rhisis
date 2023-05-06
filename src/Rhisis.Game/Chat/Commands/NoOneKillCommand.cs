using Microsoft.Extensions.Logging;
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Protocol.Snapshots;

namespace Rhisis.Game.Chat.Commands;

[ChatCommand("/noonekill", AuthorityType.GameMaster)]
[ChatCommand("/nook", AuthorityType.GameMaster)]
internal sealed class NoOneKillCommand : IChatCommand
{
    private readonly ILogger<NoOneKillCommand> _logger;

    /// <summary>
    /// Creates a new <see cref="NoOneKillCommand"/> instance.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public NoOneKillCommand(ILogger<NoOneKillCommand> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void Execute(Player player, object[] parameters)
    {
        if (player.Mode.HasFlag(ModeType.ONEKILL_MODE))
        {
            player.Mode &= ~ModeType.ONEKILL_MODE;

            using (var snapshot = new ModifyModeSnapshot(player, player.Mode))
            {
                player.Send(snapshot);
                player.SendToVisible(snapshot);
            }

            _logger.LogTrace($"Player '{player.Name}' is not anymore in OneKill mode.");
        }
        else
        {
            _logger.LogTrace($"Player '{player.Name}' is currently not in OneKill mode.");
        }
    }
}
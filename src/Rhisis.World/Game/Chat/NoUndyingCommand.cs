using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.Game.Common;
using Rhisis.Game.Abstractions.Features.Chat;
using Rhisis.Game.Abstractions.Entities;
using Rhisis.Network.Snapshots;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/noundying", AuthorityType.GameMaster)]
    [ChatCommand("/noud", AuthorityType.GameMaster)]
    public class NoUndyingChatCommand : IChatCommand
    {
        private readonly ILogger<NoUndyingChatCommand> _logger;
        private readonly IPlayerDataPacketFactory _playerDataPacketFactory;

        /// <summary>
        /// Creates a new <see cref="NoUndyingChatCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="playerDataPacketFactory">Player data packey factory system.</param>
        public NoUndyingChatCommand(ILogger<NoUndyingChatCommand> logger, IPlayerDataPacketFactory playerDataPacketFactory)
        {
            _logger = logger;
            _playerDataPacketFactory = playerDataPacketFactory;
        }

        /// <inheritdoc />
        public void Execute(IPlayer player, object[] parameters)
        {
            if (player.Mode.HasFlag(ModeType.MATCHLESS_MODE))
            {
                player.Mode &= ~ModeType.MATCHLESS_MODE;

                using (var snapshot = new ModifyModeSnapshot(player, player.Mode))
                {
                    player.Send(snapshot);
                    player.SendToVisible(snapshot);
                }

                _logger.LogTrace($"Player '{player.Name}' is not anymore in undying mode.");
            }
            else
            {
                _logger.LogTrace($"Player '{player.Name}' is currently not in undying mode.");
            }
        }
    }
}
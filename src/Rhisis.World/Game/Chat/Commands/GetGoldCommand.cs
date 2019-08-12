using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.PlayerData;
using System;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/getgold", AuthorityType.Administrator)]
    [ChatCommand("/gg", AuthorityType.Administrator)]
    public class GetGoldChatCommand : IChatCommand
    {
        private readonly ILogger<GetGoldChatCommand> _logger;
        private readonly IPlayerDataSystem _playerDataSystem;

        public GetGoldChatCommand(ILogger<GetGoldChatCommand> logger, IPlayerDataSystem playerDataSystem)
        {
            this._logger = logger;
            this._playerDataSystem = playerDataSystem;
        }

        public void Execute(IPlayerEntity player, object[] parameters)
        {
            if (parameters.Length == 1)
            {
                int gold = Convert.ToInt32(parameters[0]);

                //if (int.TryParse(parameters[0], out int GoldByCommand))
                //{
                //    player.PlayerData.Gold += GoldByCommand;
                //    WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                //}
            }
        }
    }
}
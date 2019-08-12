using NLog;
using Rhisis.Core.Common;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.Teleport;
using System;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/tp", AuthorityType.GameMaster)]
    [ChatCommand("/teleport", AuthorityType.GameMaster)]
    public class TeleportationCommand
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void TeleportCommand(IPlayerEntity player, string[] parameters)
        {
            switch (parameters.Length)
            {
                case 2: // when you write 2 parameters in the command
                    TeleportCommandTwoParam(player, parameters);
                    break;
                case 3: // when you write 3 parameters in the command
                    TeleportCommandThreeParam(player, parameters);
                    break;
                case 4: // when you write 4 parameters in the command
                    TeleportCommandFourParam(player, parameters);
                    break;
                default: // when you write more than 4 or less than 2 parameters in the command
                    Logger.Error("Chat: /teleport command must have 2, 3 or 4 parameters.");
                    return;
            }
        }

        private void TeleportCommandTwoParam(IPlayerEntity player, string[] parameters)
        {
            if (!float.TryParse(parameters[0], out float posXValue) || !float.TryParse(parameters[1], out float posZValue))
            {
                throw new ArgumentException("You must write numbers for teleport command's parameters");
            }

            SystemManager.Instance.Execute<TeleportSystem>(player, new TeleportEventArgs(player.Object.MapId, posXValue, posZValue));
        }

        private void TeleportCommandThreeParam(IPlayerEntity player, string[] parameters)
        {
            if (!ushort.TryParse(parameters[0], out ushort mapIdValue) || !float.TryParse(parameters[1], out float posXValue)
                || !float.TryParse(parameters[2], out float posZValue))
            {
                throw new ArgumentException("You must write numbers for teleport command's parameters");
            }


            SystemManager.Instance.Execute<TeleportSystem>(player, new TeleportEventArgs(mapIdValue, posXValue, posZValue));
        }

        private void TeleportCommandFourParam(IPlayerEntity player, string[] parameters)
        {
            if (!ushort.TryParse(parameters[0], out ushort mapIdValue) || !float.TryParse(parameters[1], out float posXValue)
            || !float.TryParse(parameters[2], out float posYValue) || !float.TryParse(parameters[3], out float posZValue))
            {
                throw new ArgumentException("You must write numbers for teleport command's parameters");
            }

            SystemManager.Instance.Execute<TeleportSystem>(player, new TeleportEventArgs(mapIdValue, posXValue, posZValue));
        }
    }
}
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.Teleport;
using System;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/tp", AuthorityType.GameMaster)]
    [ChatCommand("/teleport", AuthorityType.GameMaster)]
    public class TeleportationCommand : IChatCommand
    {
        private readonly ILogger<TeleportationCommand> _logger;
        private readonly ITeleportSystem _teleportSystem;

        /// <summary>
        /// Creates a new <see cref="TeleportationCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="teleportSystem">Teleport system.</param>
        public TeleportationCommand(ILogger<TeleportationCommand> logger, ITeleportSystem teleportSystem)
        {
            this._logger = logger;
            this._teleportSystem = teleportSystem;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {
            int destinationMapId = player.Object.MapId;
            var destinationPosition = new Vector3();

            switch (parameters.Length)
            {
                case 2:
                    destinationPosition.X = Convert.ToSingle(parameters[0]);
                    destinationPosition.Z = Convert.ToSingle(parameters[1]);
                    break;
                case 3:
                    destinationMapId = Convert.ToInt32(parameters[0]);
                    destinationPosition.X = Convert.ToSingle(parameters[1]);
                    destinationPosition.Z = Convert.ToSingle(parameters[2]);
                    break;
                case 4:
                    destinationMapId = Convert.ToInt32(parameters[0]);
                    destinationPosition.X = Convert.ToSingle(parameters[1]);
                    destinationPosition.Y = Convert.ToSingle(parameters[2]);
                    destinationPosition.Z = Convert.ToSingle(parameters[3]);
                    break;
                default: throw new ArgumentException("Too many or not enough arguments.");
            }

            this._teleportSystem.Teleport(player, destinationMapId, destinationPosition.X, destinationPosition.Y, destinationPosition.Z, player.Object.Angle);
        }
    }
}
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Maps;
using System;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Maps.Regions;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/createmonster", AuthorityType.Administrator)]
    [ChatCommand("/cn", AuthorityType.Administrator)]
    [ChatCommand("/monster", AuthorityType.Administrator)]
    public class CreateMonsterChatCommand : IChatCommand
    {
        private readonly ILogger<CreateMonsterChatCommand> _logger;
        private readonly IMonsterFactory _monsterFactory;
        private readonly IMapManager _mapManager;

        /// <summary>
        /// Creates a new <see cref="CreateMonsterChatCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="monsterFactory">Monster factory.</param>
        /// <param name="mapManager">Map Manager.</param>

        public CreateMonsterChatCommand(ILogger<CreateMonsterChatCommand> logger, IMapManager mapManager, IMonsterFactory monsterFactory)
        {
            this._logger = logger;
            this._mapManager = mapManager;
            this._monsterFactory = monsterFactory;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {

            if (parameters.Length <= 0 || parameters.Length > 2)
            {
                throw new ArgumentException($"Create monster command must have 1 or 2 parameters.", nameof(parameters));
            }

            if (!int.TryParse((string)parameters[0], out int monsterId)) 
            {
                throw new ArgumentException($"Cannot convert the first parameter in int.", nameof(parameters));
            }

            int quantityToSpawn = 1;

            if (parameters.Length == 2) 
            {
                if (!int.TryParse((string)parameters[1], out quantityToSpawn)) 
                {
                    throw new ArgumentException($"Cannot convert the second parameter in int.", nameof(parameters));
                }
            }
            this._logger.LogTrace($"{player.Object.Name} want to spawn {quantityToSpawn} mmonster");

            const int sizeOfSpawnArea = 13;    
            const int respawnTime = 65535;
            IMapInstance currentMap = player.Object.CurrentMap;
            IMapLayer currentMapLayer = currentMap.GetMapLayer(player.Object.LayerId);
            Vector3 currentPosition = player.Object.Position.Clone();
            var respawnRegion = new MapRespawnRegion((int)currentPosition.X-sizeOfSpawnArea/2, (int)currentPosition.Z-sizeOfSpawnArea/2, sizeOfSpawnArea, sizeOfSpawnArea, respawnTime , WorldObjectType.Mover, monsterId, quantityToSpawn);
            for (int i = 0; i < quantityToSpawn; i++) 
            {
                IMonsterEntity monsterToCreate = this._monsterFactory.CreateMonster(currentMap, currentMapLayer, monsterId, respawnRegion, true);
                monsterToCreate.Object.Position = player.Object.Position.Clone();
                currentMapLayer.AddEntity(monsterToCreate);
            }
        }
    }
}
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using System;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.Core.Resources;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/createmonster", AuthorityType.Administrator)]
    [ChatCommand("/cn", AuthorityType.Administrator)]
    [ChatCommand("/monster", AuthorityType.Administrator)]
    public class CreateMonsterChatCommand : IChatCommand
    {
        private readonly IGameResources _gameResources;
        private readonly ILogger<CreateMonsterChatCommand> _logger;
        private readonly IMonsterFactory _monsterFactory;
        private readonly ITextPacketFactory _textPacketFactory;
        private readonly IMapManager _mapManager;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new <see cref="CreateMonsterChatCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="monsterFactory">Monster factory.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        /// <param name="mapManager">Map Manager.</param>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="serviceProvider">Game resources.</param>
       

        public CreateMonsterChatCommand(ILogger<CreateMonsterChatCommand> logger, IMapManager mapManager, IMonsterFactory monsterFactory, ITextPacketFactory textPacketFactory, IServiceProvider serviceProvider, IGameResources gameResources)
        {
            this._logger = logger;
            this._mapManager = mapManager;
            this._monsterFactory = monsterFactory;
            this._textPacketFactory = textPacketFactory;
            this._serviceProvider = serviceProvider;
            this._gameResources = gameResources;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {
            if (parameters.Length <= 0 || parameters.Length > 2)
            {
                throw new ArgumentException($"Create monster command must have 1 or 2 parameters.", nameof(parameters));
            }

            if (!Int32.TryParse((string)parameters[0], out int monsterId)) {
                throw new ArgumentException($"Cannot convert '{parameters[0]}' in int.");
            }

            int quantityToSpawn = 1;

            if ( parameters.Length == 2) {
                if (!Int32.TryParse((string)parameters[1], out quantityToSpawn)) {
                    throw new ArgumentException($"Cannot convert '{parameters[1]}' in int.");
                }
            }

            const int sizeOfSpawnArea = 12;    
            IMapInstance currentMap = player.Object.CurrentMap;
            IMapLayer currentMapLayer = currentMap.GetMapLayer(player.Object.LayerId);
            Vector3 currentPosition = player.Object.Position.Clone();
            var respawnRegion = new MapRespawnRegion((int)currentPosition.X-sizeOfSpawnArea/2, (int)currentPosition.Z-sizeOfSpawnArea/2, sizeOfSpawnArea, sizeOfSpawnArea, 0 , WorldObjectType.Mover, monsterId, quantityToSpawn);
            IMonsterEntity monsterToCreate = this._monsterFactory.CreateMonster(currentMap, currentMapLayer, monsterId, respawnRegion);
            this._logger.LogDebug($"Administrator {player.Object.Name} is creating {quantityToSpawn} {monsterToCreate.Object.Name}");  
            for (int i = 0; i < quantityToSpawn; i++) {
                IMonsterEntity monsterToSpawn = this._monsterFactory.DuplicateMonster(monsterToCreate, currentPosition, true);
                currentMapLayer.AddEntity(monsterToSpawn);
            }
        }
    }
}
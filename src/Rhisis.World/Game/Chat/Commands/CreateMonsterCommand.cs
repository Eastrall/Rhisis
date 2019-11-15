using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using System;
using Rhisis.World.Systems;
using Rhisis.Core.Structures;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/createmonster", AuthorityType.Administrator)]
    [ChatCommand("/cn", AuthorityType.Administrator)]
    [ChatCommand("/monster", AuthorityType.Administrator)]
    public class CreateMonsterChatCommand : IChatCommand
    {
        private readonly ILogger<CreateMonsterChatCommand> _logger;
        private readonly IMonsterFactory _monsterFactory;
        private readonly ITextPacketFactory _textPacketFactory;
        private readonly IMapManager _mapManager;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new <see cref="CreateMonsterChatCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="mapManager">Map Manager.</param>
        /// <param name="worldSpawnPacketFactory">World spawn packet factory.</param>
        /// <param name="monsterFactory">Monster factory.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public CreateMonsterChatCommand(ILogger<CreateMonsterChatCommand> logger, IMapManager mapManager, IMonsterFactory monsterFactory, ITextPacketFactory textPacketFactory, IServiceProvider serviceProvider, IRespawnSystem respawn)
        {
            this._logger = logger;
            this._mapManager = mapManager;
            this._monsterFactory = monsterFactory;
            this._textPacketFactory = textPacketFactory;
            this._serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {
            if (parameters.Length <= 0)
            {
                throw new ArgumentException($"Create monster command must have at least one parameter.", nameof(parameters));
            }

            IMapInstance currentMap = player.Object.CurrentMap;
            IMapLayer currentMapLayer = currentMap.GetMapLayer(player.Object.LayerId);
            var currentPosition = new Vector3();
            currentPosition.X = player.Object.Position.X;
            currentPosition.Y = player.Object.Position.Y;
            currentPosition.Z = player.Object.Position.Z;
            int sizeOfSpawnArea = 12; 
            var simulatedRegion = new Rectangle((int)player.Object.Position.X-sizeOfSpawnArea/2, (int)player.Object.Position.Z-sizeOfSpawnArea/2, sizeOfSpawnArea, sizeOfSpawnArea);
            int monsterId = Convert.ToInt32(parameters[0]);
            int quantityToSpawn = parameters.Length >= 2 ? Convert.ToInt32(parameters[1]) : 1;   


            for (int i = 0; i < quantityToSpawn; i++) {
                IMonsterEntity monsterToCreate = this._monsterFactory.CreateMonster(currentMap, currentMapLayer, monsterId, currentPosition);
                monsterToCreate.Object.IsSummoned = true;
                monsterToCreate.Rectangle = simulatedRegion;
                currentMapLayer.AddEntity(monsterToCreate);
                this._logger.LogDebug($"Administrator {player.Object.Name} is creating a {monsterToCreate.Object.Name}");  
            }
        }
    }
}
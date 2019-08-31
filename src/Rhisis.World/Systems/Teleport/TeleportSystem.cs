using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Visibility;

namespace Rhisis.World.Systems.Teleport
{
    [Injectable]
    public sealed class TeleportSystem : ITeleportSystem
    {
        private readonly ILogger<TeleportSystem> _logger;
        private readonly IMapManager _mapManager;
        private readonly IVisibilitySystem _visibilitySystem;
        private readonly IPlayerPacketFactory _playerPacketFactory;
        private readonly IWorldSpawnPacketFactory _worldSpawnPacketFactory;
        private readonly ITextPacketFactory _textPacketFactory;

        /// <summary>
        /// Creates a new <see cref="TeleportSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="mapManager">Map manager.</param>
        /// <param name="visibilitySystem">Visibility System.</param>
        /// <param name="playerPacketFactory">Player packet factory.</param>
        /// <param name="worldSpawnPacketFactory">World spawn packet factory.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public TeleportSystem(ILogger<TeleportSystem> logger, IMapManager mapManager, IVisibilitySystem visibilitySystem, IPlayerPacketFactory playerPacketFactory, IWorldSpawnPacketFactory worldSpawnPacketFactory, ITextPacketFactory textPacketFactory)
        {
            this._logger = logger;
            this._mapManager = mapManager;
            this._visibilitySystem = visibilitySystem;
            this._playerPacketFactory = playerPacketFactory;
            this._worldSpawnPacketFactory = worldSpawnPacketFactory;
            this._textPacketFactory = textPacketFactory;
        }

        /// <inheritdoc />
        public void Teleport(IPlayerEntity player, int mapId, float x, float? y, float z, float angle = 0)
        {
            if (player.Object.MapId != mapId)
            {
                IMapInstance destinationMap = this._mapManager.GetMap(mapId);

                if (destinationMap == null)
                {
                    this._logger.LogError($"Cannot find map with id '{mapId}'.");
                    this._textPacketFactory.SendSnoop(player, $"Cannot find map with id '{mapId}'.");
                    return;
                }

                if (!destinationMap.ContainsPosition(new Vector3(x, 0, z)))
                {
                    this._logger.LogError($"Cannot teleport. Destination position is out of map bounds.");
                    return;
                }

                this._visibilitySystem.DespawnEntity(player);
                player.Object.Spawned = false;
                player.Object.CurrentMap = this._mapManager.GetMap(destinationMap.Id);
                player.Object.MapId = destinationMap.Id;
                player.Object.LayerId = destinationMap.DefaultMapLayer.Id;

                // TODO: get map height at x/z position
                float positionY = y ?? 100;
                player.Object.Position = new Vector3(x, positionY, z);
                player.Moves.DestinationPosition = player.Object.Position.Clone();

                this._playerPacketFactory.SendPlayerReplace(player);
                this._worldSpawnPacketFactory.SendPlayerSpawn(player);
                player.Object.Spawned = true;
            }
            else
            {
                if (!player.Object.CurrentMap.ContainsPosition(new Vector3(x, 0, z)))
                {
                    this._logger.LogError($"Cannot teleport. Destination position is out of map bounds.");
                    return;
                }

                // TODO: get map height at x/z position
                float positionY = y ?? 100;
                player.Object.Position = new Vector3(x, positionY, z);
                player.Moves.DestinationPosition = player.Object.Position.Clone();
            }

            this._playerPacketFactory.SendPlayerTeleport(player);
        }
    }
}

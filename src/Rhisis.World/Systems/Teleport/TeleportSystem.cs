using Microsoft.Extensions.Logging;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Teleport
{
    /// <summary>
    /// Teleport system used to teleport player to a target map at a given position.
    /// </summary>
    [System(SystemType.Notifiable)]
    public sealed class TeleportSystem : ISystem
    {
        private readonly ILogger<TeleportSystem> _logger;
        private readonly IMapManager _mapManager;

        /// <summary>
        /// Creates a new <see cref="TeleportSystem"/> instance.
        /// </summary>
        public TeleportSystem(ILogger<TeleportSystem> logger, IMapManager mapManager)
        {
            this._logger = logger;
            this._mapManager = mapManager;
        }

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
            {
                this._logger.LogError($"Cannot execute {nameof(TeleportSystem)}. {entity.Object.Name} is not a player.");
                return;
            }

            if (!args.GetCheckArguments())
            {
                this._logger.LogError($"Cannot execute {nameof(TeleportSystem)} action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case TeleportEventArgs e:
                    this.Teleport(player, e);
                    break;
            }
        }

        /// <summary>
        /// Teleports the player to the given map and position.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="e">Teleport args.</param>
        private void Teleport(IPlayerEntity player, TeleportEventArgs e)
        {
            if (player.Object.MapId != e.MapId)
            {
                IMapInstance destinationMap = this._mapManager.GetMap(e.MapId);

                if (destinationMap == null)
                {
                    this._logger.LogError($"Cannot find map with id '{e.MapId}'.");
                    WorldPacketFactory.SendSnoop(player, $"Cannot find map with id '{e.MapId}'.");
                    return;
                }

                if (!destinationMap.ContainsPosition(new Vector3(e.PositionX, 0, e.PositionZ)))
                {
                    this._logger.LogError($"Cannot teleport. Destination position is out of map bounds.");
                    return;
                }

                IMapLayer defaultMapLayer = destinationMap.GetDefaultMapLayer();
                //player.SwitchContext(defaultMapLayer);
                player.Object.Spawned = false;
                player.Object.CurrentMap = this._mapManager.GetMap(destinationMap.Id);
                player.Object.MapId = destinationMap.Id;
                player.Object.LayerId = defaultMapLayer.Id;

                // TODO: get map height at x/z position
                float positionY = e.PositionY ?? 100;
                player.Object.Position = new Vector3(e.PositionX, positionY, e.PositionZ);
                player.Moves.DestinationPosition = player.Object.Position.Clone();

                WorldPacketFactory.SendReplaceObject(player);
                WorldPacketFactory.SendPlayerSpawn(player);
                player.Object.Spawned = true;
            }
            else
            {
                if (!player.Object.CurrentMap.ContainsPosition(new Vector3(e.PositionX, 0, e.PositionZ)))
                {
                    this._logger.LogError($"Cannot teleport. Destination position is out of map bounds.");
                    return;
                }

                // TODO: get map height at x/z position
                float positionY = e.PositionY ?? 100;
                player.Object.Position = new Vector3(e.PositionX, positionY, e.PositionZ);
                player.Moves.DestinationPosition = player.Object.Position.Clone();
            }

            WorldPacketFactory.SendPlayerTeleport(player);
        }
    }
}

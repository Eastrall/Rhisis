using Microsoft.Extensions.Logging;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Game.Factories;
using Rhisis.World.Packets;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class JoinGameHandler
    {
        private readonly ILogger<JoinGameHandler> _logger;
        private readonly IDatabase _database;
        private readonly IPlayerFactory _playerFactory;
        private readonly IWorldSpawnPacketFactory _worldSpawnPacketFactory;

        /// <summary>
        /// Creates a new <see cref="JoinGameHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="database">Database access layer.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="mapManager">Map manager.</param>
        /// <param name="behaviorManager">Behavior manager.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="worldSpawnPacketFactory">World spawn packet factory.</param>
        public JoinGameHandler(ILogger<JoinGameHandler> logger, IDatabase database, IPlayerFactory playerFactory, IWorldSpawnPacketFactory worldSpawnPacketFactory)
        {
            this._logger = logger;
            this._database = database;
            this._playerFactory = playerFactory;
            this._worldSpawnPacketFactory = worldSpawnPacketFactory;
        }

        /// <summary>
        /// Prepares the player to join the world.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming join packet.</param>
        [HandlerAction(PacketType.JOIN)]
        public void OnJoin(IWorldClient client, JoinPacket packet)
        {
            DbCharacter character = this._database.Characters.Get(packet.PlayerId);

            if (character == null)
            {
                this._logger.LogError($"Invalid player id received from client; cannot find player with id: {packet.PlayerId}");
                return;
            }

            if (character.IsDeleted)
            {
                this._logger.LogWarning($"Cannot connect with character '{character.Name}' for user '{character.User.Username}'. Reason: character is deleted.");
                return;
            }

            if (character.User.Authority <= 0)
            {
                this._logger.LogWarning($"Cannot connect with '{character.Name}'. Reason: User {character.User.Username} is banned.");
                // TODO: send error to client
                return;
            }

            // 1st: Create the player entity based on the character informations from database.
            client.Player = this._playerFactory.CreatePlayer(character);

            // 2nd: Set the connection component
            client.Player.Connection = client;

            // 3rd: spawn the player
            this._worldSpawnPacketFactory.SendPlayerSpawn(client.Player);

            // 4th: player is now spawned
            client.Player.Object.Spawned = true;
            client.Player.PlayerData.LoggedInAt = DateTime.UtcNow;
        }
    }
}

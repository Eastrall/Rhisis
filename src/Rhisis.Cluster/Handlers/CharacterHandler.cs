using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rhisis.Cluster.Client;
using Rhisis.Cluster.Packets;
using Rhisis.Core.Common.Formulas;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Game;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Core;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.Cluster;
using Sylver.HandlerInvoker.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Rhisis.Cluster.WorldCluster.Server;

namespace Rhisis.Cluster.Handlers
{
    [Handler]
    public class CharacterHandler
    {
        private readonly ILogger<CharacterHandler> _logger;
        private readonly IRhisisDatabase _database;
        private readonly IClusterServer _clusterServer;
        private readonly ICache<int, WorldServerInfo> _worldCache;
        private readonly IGameResources _gameResources;
        private readonly IClusterPacketFactory _clusterPacketFactory;

        /// <summary>
        /// Creates a new <see cref="CharacterHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="database">Rhisis database.</param>
        /// <param name="clusterServer">Cluster server instance.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="clusterPacketFactory">Cluster server packet factory.</param>
        /// <param name="worldCache">World server information cache</param>
        public CharacterHandler(ILogger<CharacterHandler> logger, IRhisisDatabase database, IClusterServer clusterServer, 
            IGameResources gameResources, IClusterPacketFactory clusterPacketFactory, ICache<int, WorldServerInfo> worldCache)
        {
            _logger = logger;
            _database = database;
            _clusterServer = clusterServer;
            _gameResources = gameResources;
            _clusterPacketFactory = clusterPacketFactory;
            _worldCache = worldCache;
        }

        /// <summary>
        /// Handles the GETPLAYERLIST packet and sends the user's characters list to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.GETPLAYERLIST)]
        public void OnGetPlayerList(IClusterClient client, GetPlayerListPacket packet)
        {
            WorldServerInfo selectedWorldServer = _worldCache.TryGetOrDefault(packet.ServerId);

            if (selectedWorldServer == null)
            {
                _logger.LogWarning($"Unable to get characters list for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: client requested the list on a not connected World server.");
                client.Disconnect();
                return;
            }

            DbUser dbUser = _database.Users.FirstOrDefault(x => x.Username == packet.Username);

            if (dbUser == null)
            {
                _logger.LogWarning($"[SECURITY] Unable to load character list for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            IEnumerable<DbCharacter> userCharacters = GetCharacters(dbUser.Id);

            _clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, userCharacters);
            _clusterPacketFactory.SendWorldAddress(client, selectedWorldServer.Host);

            if (_clusterServer.ClusterConfiguration.EnableLoginProtect)
                _clusterPacketFactory.SendLoginNumPad(client, client.LoginProtectValue);
        }

        /// <summary>
        /// Creates a character.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.CREATE_PLAYER)]
        public void OnCreatePlayer(IClusterClient client, CreatePlayerPacket packet)
        {
            DbUser dbUser = _database.Users.FirstOrDefault(x => x.Username == packet.Username && x.Password == packet.Password);

            if (dbUser == null)
            {
                _logger.LogWarning($"[SECURITY] Unable to create new character for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            if (_database.Characters.Any(x => x.Name == packet.CharacterName))
            {
                _logger.LogWarning(
                        $"Unable to create new character for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                        $"Reason: character name '{packet.CharacterName}' already exists.");

                _clusterPacketFactory.SendClusterError(client, ErrorType.USER_EXISTS);
                return;
            }

            DefaultCharacter defaultCharacter = _clusterServer.ClusterConfiguration.DefaultCharacter;
            DefaultStartItems defaultEquipment = packet.Gender == 0 ? defaultCharacter.Man : defaultCharacter.Woman;

            if (!_gameResources.Jobs.TryGetValue(packet.Job, out JobData jobData))
            {
                _logger.LogError($"Cannot find job data for job '{packet.Job}' for user '{dbUser.Username}'.");
                client.Disconnect();
                return;
            }

            var newCharacter = new DbCharacter()
            {
                UserId = dbUser.Id,
                Name = packet.CharacterName,
                Slot = (byte)packet.Slot,
                SkinSetId = packet.SkinSet,
                HairColor = (int)packet.HairColor,
                FaceId = packet.HeadMesh,
                HairId = packet.HairMeshId,
                BankCode = packet.BankPassword,
                Gender = packet.Gender,
                JobId = (int)packet.Job,
                Hp = HealthFormulas.GetMaxOriginHp(defaultCharacter.Level, defaultCharacter.Stamina,
                    jobData.MaxHpFactor),
                Mp = HealthFormulas.GetMaxOriginMp(defaultCharacter.Level, defaultCharacter.Intelligence,
                    jobData.MaxMpFactor, true),
                Fp = HealthFormulas.GetMaxOriginFp(defaultCharacter.Level, defaultCharacter.Stamina,
                    defaultCharacter.Dexterity, defaultCharacter.Strength, jobData.MaxFpFactor, true),
                Strength = defaultCharacter.Strength,
                Stamina = defaultCharacter.Stamina,
                Dexterity = defaultCharacter.Dexterity,
                Intelligence = defaultCharacter.Intelligence,
                MapId = defaultCharacter.MapId,
                PosX = defaultCharacter.PosX,
                PosY = defaultCharacter.PosY,
                PosZ = defaultCharacter.PosZ,
                Level = defaultCharacter.Level,
                Gold = defaultCharacter.Gold,
                StatPoints = 0, //TODO: create default stat point constant.
                SkillPoints = 0, //TODO: create default skill point constant.
                Experience = 0,
                ClusterId = _clusterServer.ClusterConfiguration.Id
            };

            //TODO: create game constants for slot.
            newCharacter.Items.Add(new DbInventoryItem
            {
                ItemId = defaultEquipment.StartSuit,
                Slot = 44
            });
            newCharacter.Items.Add(new DbInventoryItem
            {
                ItemId = defaultEquipment.StartHand,
                Slot = 46
            });
            newCharacter.Items.Add(new DbInventoryItem
            {
                ItemId = defaultEquipment.StartShoes,
                Slot = 47
            });
            newCharacter.Items.Add(new DbInventoryItem
            {
                ItemId = defaultEquipment.StartWeapon,
                Slot = 52
            });

            _database.Characters.Add(newCharacter);
            _database.SaveChanges();

            _logger.LogInformation($"Character '{newCharacter.Name}' has been created successfully for user '{dbUser.Username}' from {client.Socket.RemoteEndPoint}.");

            IEnumerable<DbCharacter> dbCharacters = GetCharacters(dbUser.Id);

            _clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, dbCharacters);
        }

        /// <summary>
        /// Deletes a character.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.DEL_PLAYER)]
        public void OnDeletePlayer(IClusterClient client, DeletePlayerPacket packet)
        {
            DbUser dbUser = _database.Users.FirstOrDefault(x => x.Username == packet.Username && x.Password == packet.Password);

            if (dbUser == null)
            {
                _logger.LogWarning($"[SECURITY] Unable to create new character for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            if (!string.Equals(packet.Password, packet.PasswordConfirmation, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: passwords entered do not match.");
                _clusterPacketFactory.SendClusterError(client, ErrorType.WRONG_PASSWORD);
                return;
            }

            DbCharacter characterToDelete = _database.Characters.FirstOrDefault(x => x.Id == packet.CharacterId);

            // Check if character exist.
            if (characterToDelete == null)
            {
                _logger.LogWarning($"[SECURITY] Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: user doesn't have any character with this id.");
                client.Disconnect();
                return;
            }

            if (characterToDelete.IsDeleted)
            {
                _logger.LogWarning($"[SECURITY] Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                       "Reason: character is already deleted.");
                return;
            }

            characterToDelete.IsDeleted = true;

            _database.Characters.Update(characterToDelete);
            _database.SaveChanges();

            _logger.LogInformation($"Character '{characterToDelete.Name}' has been deleted successfully for user '{packet.Username}' from {client.Socket.RemoteEndPoint}.");

            IEnumerable<DbCharacter> dbCharacters = GetCharacters(dbUser.Id);

            _clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, dbCharacters);
        }

        /// <summary>
        /// Prepare the character to join the world server.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.PRE_JOIN)]
        public void OnPreJoin(IClusterClient client, PreJoinPacket packet)
        {
            DbCharacter character = _database.Characters.FirstOrDefault(x => x.Id == packet.CharacterId);

            if (character == null)
            {
                _logger.LogWarning($"[SECURITY] Unable to prejoin character id '{packet.CharacterName}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                      $"Reason: no character with id {packet.CharacterId}.");
                client.Disconnect();
                return;
            }

            if (character.IsDeleted)
            {
                _logger.LogWarning($"[SECURITY] Unable to prejoin with character '{character.Name}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                                "Reason: character is deleted.");
                client.Disconnect();
                return;
            }

            if (character.Name != packet.CharacterName)
            {
                _logger.LogWarning($"[SECURITY] Unable to prejoin character '{character.Name}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                       "Reason: character is not owned by this user.");
                client.Disconnect();
                return;
            }

            if (_clusterServer.ClusterConfiguration.EnableLoginProtect &&
                LoginProtect.GetNumPadToPassword(client.LoginProtectValue, packet.BankCode) != character.BankCode)
            {
                _logger.LogWarning($"Unable to prejoin character '{character.Name}' for user '{packet.Username}' from {client.Socket.RemoteEndPoint}. " +
                    "Reason: bad bank code.");
                client.LoginProtectValue = new Random().Next(0, 1000);
                _clusterPacketFactory.SendLoginProtect(client, client.LoginProtectValue);
                return;
            }

            _clusterPacketFactory.SendJoinWorld(client);
            _logger.LogInformation($"Character '{character.Name}' has prejoin successfully the game for user '{packet.Username}' from {client.Socket.RemoteEndPoint}.");
        }

        /// <summary>
        /// Gets the characters of a given user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Collection of <see cref="DbCharacter"/>.</returns>
        private IEnumerable<DbCharacter> GetCharacters(int userId)
        {
            return _database.Characters.AsNoTracking()
                .Include(x => x.Items)
                    .ThenInclude(x => x.Item)
                .Where(x => x.UserId == userId && !x.IsDeleted);
        }
    }
}

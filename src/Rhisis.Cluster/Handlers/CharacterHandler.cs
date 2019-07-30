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

namespace Rhisis.Cluster.Handlers
{
    [Handler]
    public class CharacterHandler
    {
        private readonly ILogger<CharacterHandler> _logger;
        private readonly IDatabase _database;
        private readonly IClusterServer _clusterServer;
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
        public CharacterHandler(ILogger<CharacterHandler> logger, IDatabase database, IClusterServer clusterServer, IGameResources gameResources, IClusterPacketFactory clusterPacketFactory)
        {
            this._logger = logger;
            this._database = database;
            this._clusterServer = clusterServer;
            this._gameResources = gameResources;
            this._clusterPacketFactory = clusterPacketFactory;
        }

        /// <summary>
        /// Handles the GETPLAYERLIST packet and sends the user's characters list to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.GETPLAYERLIST)]
        public void OnGetPlayerList(IClusterClient client, GetPlayerListPacket packet)
        {
            WorldServerInfo selectedWorldServer = this._clusterServer.GetWorldServerById(packet.ServerId);

            if (selectedWorldServer == null)
            {
                this._logger.LogWarning($"Unable to get characters list for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: client requested the list on a not connected World server.");
                client.Disconnect();
                return;
            }

            DbUser dbUser = this._database.Users.GetUser(packet.Username);

            if (dbUser == null)
            {
                this._logger.LogWarning($"[SECURITY] Unable to load character list for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            IEnumerable<DbCharacter> userCharacters = this._database.Characters.GetCharacters(dbUser.Id);

            this._clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, userCharacters);
            this._clusterPacketFactory.SendWorldAddress(client, selectedWorldServer.Host);

            if (this._clusterServer.ClusterConfiguration.EnableLoginProtect)
                this._clusterPacketFactory.SendLoginNumPad(client, client.LoginProtectValue);
        }

        /// <summary>
        /// Creates a character.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.CREATE_PLAYER)]
        public void OnCreatePlayer(IClusterClient client, CreatePlayerPacket packet)
        {
            DbUser dbUser = this._database.Users.GetUser(packet.Username, packet.Password);

            if (dbUser == null)
            {
                this._logger.LogWarning($"[SECURITY] Unable to create new character for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            if (this._database.Characters.HasAny(x => x.Name == packet.Name))
            {
                this._logger.LogWarning(
                        $"Unable to create new character for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                        $"Reason: character name '{packet.Name}' already exists.");

                this._clusterPacketFactory.SendClusterError(client, ErrorType.USER_EXISTS);
                return;
            }

            DefaultCharacter defaultCharacter = this._clusterServer.ClusterConfiguration.DefaultCharacter;
            DefaultStartItems defaultEquipment = packet.Gender == 0 ? defaultCharacter.Man : defaultCharacter.Woman;

            if (!this._gameResources.Jobs.TryGetValue(packet.Job, out JobData jobData))
            {
                this._logger.LogError($"Cannot find job data for job '{packet.Job}' for user '{dbUser.Username}'.");
                client.Disconnect();
                return;
            }

            var newCharacter = new DbCharacter()
            {
                UserId = dbUser.Id,
                Name = packet.Name,
                Slot = packet.Slot,
                SkinSetId = packet.SkinSet,
                HairColor = (int)packet.HairColor,
                FaceId = packet.HeadMesh,
                HairId = packet.HairMeshId,
                BankCode = packet.BankPassword,
                Gender = packet.Gender,
                ClassId = packet.Job,
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
            };

            //TODO: create game constants for slot.
            newCharacter.Items.Add(new DbItem(defaultEquipment.StartSuit, 44));
            newCharacter.Items.Add(new DbItem(defaultEquipment.StartHand, 46));
            newCharacter.Items.Add(new DbItem(defaultEquipment.StartShoes, 47));
            newCharacter.Items.Add(new DbItem(defaultEquipment.StartWeapon, 52));

            this._database.Characters.Create(newCharacter);
            this._database.Complete();

            this._logger.LogInformation($"Character '{newCharacter.Name}' has been created successfully for user '{dbUser.Username}' from {client.RemoteEndPoint}.");

            IEnumerable<DbCharacter> dbCharacters = this._database.Characters.GetCharacters(dbUser.Id);

            this._clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, dbCharacters);
        }

        /// <summary>
        /// Deletes a character.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.DEL_PLAYER)]
        public void OnDeletePlayer(IClusterClient client, DeletePlayerPacket packet)
        {
            DbUser dbUser = this._database.Users.GetUser(packet.Username, packet.Password);

            if (dbUser == null)
            {
                this._logger.LogWarning($"[SECURITY] Unable to create new character for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: bad presented credentials compared to the database.");
                client.Disconnect();
                return;
            }

            if (!string.Equals(packet.Password, packet.PasswordConfirmation, StringComparison.OrdinalIgnoreCase))
            {
                this._logger.LogWarning($"Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: passwords entered do not match.");
                this._clusterPacketFactory.SendClusterError(client, ErrorType.WRONG_PASSWORD);
                return;
            }

            DbCharacter characterToDelete = this._database.Characters.Get(packet.CharacterId);

            // Check if character exist.
            if (characterToDelete == null)
            {
                this._logger.LogWarning($"[SECURITY] Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: user doesn't have any character with this id.");
                client.Disconnect();
                return;
            }

            if (characterToDelete.IsDeleted)
            {
                this._logger.LogWarning($"[SECURITY] Unable to delete character id '{packet.CharacterId}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                       "Reason: character is already deleted.");
                return;
            }

            this._database.Characters.Delete(characterToDelete);
            this._database.Complete();

            this._logger.LogInformation($"Character '{characterToDelete.Name}' has been deleted successfully for user '{packet.Username}' from {client.RemoteEndPoint}.");

            IEnumerable<DbCharacter> dbCharacters = this._database.Characters.GetCharacters(dbUser.Id);

            this._clusterPacketFactory.SendPlayerList(client, packet.AuthenticationKey, dbCharacters);
        }

        /// <summary>
        /// Prepare the character to join the world server.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet data.</param>
        [HandlerAction(PacketType.PRE_JOIN)]
        public void OnPreJoin(IClusterClient client, PreJoinPacket packet)
        {
            DbCharacter character = this._database.Characters.GetCharacter(packet.CharacterId);

            if (character == null)
            {
                this._logger.LogWarning($"[SECURITY] Unable to prejoin character id '{packet.CharacterName}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                      $"Reason: no character with id {packet.CharacterId}.");
                client.Disconnect();
                return;
            }

            if (character.IsDeleted)
            {
                this._logger.LogWarning($"[SECURITY] Unable to prejoin with character '{character.Name}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                                "Reason: character is deleted.");
                client.Disconnect();
                return;
            }

            if (character.Name != packet.CharacterName)
            {
                this._logger.LogWarning($"[SECURITY] Unable to prejoin character '{character.Name}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                       "Reason: character is not owned by this user.");
                client.Disconnect();
                return;
            }

            if (this._clusterServer.ClusterConfiguration.EnableLoginProtect &&
                LoginProtect.GetNumPadToPassword(client.LoginProtectValue, packet.BankCode) != character.BankCode)
            {
                this._logger.LogWarning($"Unable to prejoin character '{character.Name}' for user '{packet.Username}' from {client.RemoteEndPoint}. " +
                    "Reason: bad bank code.");
                client.LoginProtectValue = new Random().Next(0, 1000);
                this._clusterPacketFactory.SendLoginProtect(client, client.LoginProtectValue);
                return;
            }

            this._clusterPacketFactory.SendJoinWorld(client);
            this._logger.LogInformation($"Character '{character.Name}' has prejoin successfully the game for user '{packet.Username}' from {client.RemoteEndPoint}.");
        }
    }
}

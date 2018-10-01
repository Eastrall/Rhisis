using Ether.Network.Packets;
using NLog;
using Rhisis.Cluster.Packets;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.ISC.Structures;
using Rhisis.Network.Packets;
using System;

namespace Rhisis.Cluster
{
    public static class ClusterHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(PacketType.PING)]
        public static void OnPing(ClusterClient client, INetPacketStream packet)
        {
            int time = 0;
            bool isTimeout = false;

            try
            {
                time = packet.Read<int>();
            }
            catch (Exception e)
            {
                Logger.Warn(e, $"Client {client.Id} timed out.");
                isTimeout = true;
            }

            if (!isTimeout)
                CommonPacketFactory.SendPong(client, time);
        }

        [PacketHandler(PacketType.GETPLAYERLIST)]
        public static void OnGetPlayerList(ClusterClient client, INetPacketStream packet)
        {
            var buildVersion = packet.Read<string>();
            var authenticationKey = packet.Read<int>();
            var username = packet.Read<string>();
            var password = packet.Read<string>();
            var serverId = packet.Read<int>();
            WorldServerInfo selectedWorldServer = ClusterServer.GetWorldServerById(serverId);

            // Check if asked World server is still connected.
            if (selectedWorldServer == null)
            {
                Logger.Warn($"Unable to get characters list for user '{username}' from {client.RemoteEndPoint}. " +
                    "Reason: client requested the list on a not connected World server.");
                client.Disconnect();
                return;
            }

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                DbUser dbUser = database.Users.Get(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                // Check if user exist.
                if (dbUser == null)
                {
                    Logger.Warn($"[SECURITY] Unable to create new character for user '{username}' from {client.RemoteEndPoint}. " +
                        "Reason: bad presented credentials compared to the database.");
                    client.Disconnect();
                    return;
                }

                Logger.Debug($"Send character list to user '{username}' from {client.RemoteEndPoint}.");
                ClusterPacketFactory.SendPlayerList(client, authenticationKey, dbUser.Characters);
                ClusterPacketFactory.SendWorldAddress(client, selectedWorldServer.Host);

                if (client.Configuration.EnableLoginProtect)
                    ClusterPacketFactory.SendLoginNumPad(client, client.LoginProtectValue);
            }
        }

        [PacketHandler(PacketType.CREATE_PLAYER)]
        public static void OnCreatePlayer(ClusterClient client, INetPacketStream packet)
        {
            var username = packet.Read<string>();
            var password = packet.Read<string>();
            var slot = packet.Read<byte>();
            var name = packet.Read<string>();
            var faceId = packet.Read<byte>();
            var costumeId = packet.Read<byte>();
            var skinSet = packet.Read<byte>();
            var hairMeshId = packet.Read<byte>();
            var hairColor = packet.Read<uint>();
            var gender = packet.Read<byte>();
            var job = packet.Read<byte>();
            var headMesh = packet.Read<byte>();
            var bankPassword = packet.Read<int>();
            var authenticationKey = packet.Read<int>();

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                DbUser dbUser = database.Users.Get(x =>
                    x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    x.Password.Equals(password, StringComparison.OrdinalIgnoreCase));

                // Check if user exist and with good password in database.
                if (dbUser == null)
                {
                    Logger.Warn($"[SECURITY] Unable to create new character for user '{username}' from {client.RemoteEndPoint}. " +
                        "Reason: bad presented credentials compared to the database.");
                    client.Disconnect();
                    return;
                }

                DbCharacter dbCharacter = database.Characters.Get(x => x.Name == name);

                // Check if character name is not already used.
                if (dbCharacter != null)
                {
                    Logger.Warn($"Unable to create new character for user '{username}' from {client.RemoteEndPoint}. " +
                        $"Reason: character name '{name}' already exists.");
                    ClusterPacketFactory.SendError(client, ErrorType.USER_EXISTS);
                    return;
                }

                DefaultCharacter defaultCharacter = client.Configuration.DefaultCharacter;
                DefaultStartItem defaultEquipment = gender == 0 ? defaultCharacter.Man : defaultCharacter.Woman;

                dbCharacter = new DbCharacter()
                {
                    UserId = dbUser.Id,
                    Name = name,
                    Slot = slot,
                    SkinSetId = skinSet,
                    HairColor = (int)hairColor,
                    FaceId = headMesh,
                    HairId = hairMeshId,
                    BankCode = bankPassword,
                    Gender = gender,
                    ClassId = job,
                    Hp = 100, //TODO: create game constants.
                    Mp = 100, //TODO: create game constants.
                    Fp = 100, //TODO: create game constants.
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
                    StatPoints = 0, //TODO: create game constants.
                    SkillPoints = 0, //TODO: create game constants.
                    Experience = 0, //TODO: create game constants.
                };

                //TODO: create game constants for slot.
                dbCharacter.Items.Add(new DbItem(defaultEquipment.StartSuit, 44));
                dbCharacter.Items.Add(new DbItem(defaultEquipment.StartHand, 46));
                dbCharacter.Items.Add(new DbItem(defaultEquipment.StartShoes, 47));
                dbCharacter.Items.Add(new DbItem(defaultEquipment.StartWeapon, 52));

                database.Characters.Create(dbCharacter);
                database.Complete();
                Logger.Info("Character '{0}' has been created successfully for user '{1}' from {2}.",
                    dbCharacter.Name, username, client.RemoteEndPoint);

                ClusterPacketFactory.SendPlayerList(client, authenticationKey, dbUser.Characters);
            }
        }

        [PacketHandler(PacketType.DEL_PLAYER)]
        public static void OnDeletePlayer(ClusterClient client, INetPacketStream packet)
        {
            var username = packet.Read<string>();
            var password = packet.Read<string>();
            var passwordConfirmation = packet.Read<string>();
            var characterId = packet.Read<int>();
            var authenticationKey = packet.Read<int>();

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                DbUser dbUser = database.Users.Get(x =>
                    x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    x.Password.Equals(password, StringComparison.OrdinalIgnoreCase));

                // Check if user exist and with good password in database.
                if (dbUser == null)
                {
                    Logger.Warn($"[SECURITY] Unable to delete character id '{characterId}' for user '{username}' from {client.RemoteEndPoint}. " +
                        "Reason: bad presented credentials compared to the database.");
                    client.Disconnect();
                    return;
                }

                // Check if given password match confirmation password.
                if (!string.Equals(password, passwordConfirmation, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Warn($"Unable to delete character id '{characterId}' for user '{username}' from {client.RemoteEndPoint}. " +
                        "Reason: passwords entered do not match.");
                    ClusterPacketFactory.SendError(client, ErrorType.WRONG_PASSWORD);
                    return;
                }

                DbCharacter dbCharacter = database.Characters.Get(characterId);

                // Check if character exist.
                if (dbCharacter == null)
                {
                    Logger.Warn($"[SECURITY] Unable to delete character id '{characterId}' for user '{username}' from {client.RemoteEndPoint}. " +
                        "Reason: user doesn't have any character with this id.");
                    client.Disconnect();
                    return;
                }

                database.Characters.Delete(dbCharacter);
                database.Complete();
                Logger.Info("Character '{0}' has been deleted successfully for user '{1}' from {2}.",
                    dbCharacter.Name, username, client.RemoteEndPoint);

                ClusterPacketFactory.SendPlayerList(client, authenticationKey, dbUser.Characters);
            }
        }

        [PacketHandler(PacketType.PRE_JOIN)]
        public static void OnPreJoin(ClusterClient client, INetPacketStream packet)
        {
            var username = packet.Read<string>();
            var characterId = packet.Read<int>();
            var characterName = packet.Read<string>();
            var bankCode = packet.Read<int>();
            DbCharacter dbCharacter = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                dbCharacter = database.Characters.Get(characterId);

            // Check if character exist.
            if (dbCharacter == null)
            {
                Logger.Warn($"[SECURITY] Unable to prejoin character id '{characterName}' for user '{username}' from {client.RemoteEndPoint}. " +
                    $"Reason: no character with id {characterId}.");
                client.Disconnect();
                return;
            }

            // Check if given username is the real owner of this character.
            if (!username.Equals(dbCharacter.User.Username, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Warn($"[SECURITY] Unable to prejoin character '{dbCharacter.Name}' for user '{username}' from {client.RemoteEndPoint}. " +
                    "Reason: character is not owned by this user.");
                client.Disconnect();
                return;
            }

            // Check if presented bank code is correct.
            if (client.Configuration.EnableLoginProtect &&
                LoginProtect.GetNumPadToPassword(client.LoginProtectValue, bankCode) != dbCharacter.BankCode)
            {
                Logger.Warn($"Unable to prejoin character '{dbCharacter.Name}' for user '{username}' from {client.RemoteEndPoint}. " +
                    "Reason: bad bank code.");
                client.LoginProtectValue = new Random().Next(0, 1000);
                ClusterPacketFactory.SendLoginProtect(client, client.LoginProtectValue);
                return;
            }

            // Finally, we connect the player.
            ClusterPacketFactory.SendJoinWorld(client);
            Logger.Info("Character '{0}' has prejoin successfully the game for user '{1}' from {2}.",
                dbCharacter.Name, username, client.RemoteEndPoint);
        }
    }
}

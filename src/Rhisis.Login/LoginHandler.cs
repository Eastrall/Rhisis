using Ether.Network.Packets;
using NLog;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Services;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Login.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using System;

namespace Rhisis.Login
{
    public static class LoginHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(PacketType.PING)]
        public static void OnPing(LoginClient client, INetPacketStream packet)
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

        [PacketHandler(PacketType.CERTIFY)]
        public static void OnLogin(LoginClient client, INetPacketStream packet)
        {
            var cryptographyService = DependencyContainer.Instance.Resolve<ICryptographyService>();
            var buildVersion = packet.Read<string>();
            var username = packet.Read<string>();
            var password = string.Empty;

            if (client.ServerConfiguration.PasswordEncryption)
            {
                byte[] passwordData = packet.ReadArray<byte>(16 * 42);
                byte[] encryptionKey = cryptographyService.BuildEncryptionKeyFromString(client.ServerConfiguration.EncryptionKey, 16);

                password = cryptographyService.Decrypt(passwordData, encryptionKey);
            }
            else
            {
                password = packet.Read<string>();
            }

            if (buildVersion != client.ServerConfiguration.BuildVersion)
            {
                Logger.Warn($"Unable to authenticate user from {client.RemoteEndPoint}. Reason: bad client build version.");
                LoginPacketFactory.SendLoginError(client, ErrorType.CERT_GENERAL);
                client.Disconnect();
                return;
            }

            DbUser dbUser = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                dbUser = database.Users.Get(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (dbUser == null)
            {
                Logger.Warn($"Unable to authenticate user from {client.RemoteEndPoint}. Reason: bad username.");
                LoginPacketFactory.SendLoginError(client, ErrorType.FLYFF_ACCOUNT);
                client.Disconnect();
                return;
            }

            if (!dbUser.Password.Equals(password, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Warn($"Unable to authenticate user from {client.RemoteEndPoint}. Reason: bad password.");
                LoginPacketFactory.SendLoginError(client, ErrorType.FLYFF_PASSWORD);
                client.Disconnect();
                return;
            }

            LoginPacketFactory.SendServerList(client, username, client.ClustersConnected);
            Logger.Info($"User '{username}' logged succesfully from {client.RemoteEndPoint}.");
        }
    }
}
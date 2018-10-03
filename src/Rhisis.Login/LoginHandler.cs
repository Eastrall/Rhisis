using Ether.Network.Packets;
using NLog;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Services;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Login.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.Login;
using System;

namespace Rhisis.Login
{
    public static class LoginHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(PacketType.PING)]
        public static void OnPing(LoginClient client, INetPacketStream packet)
        {
            var pak = new PingPacket(packet);

            if (!pak.IsTimeOut)
                CommonPacketFactory.SendPong(client, pak.Time);
        }

        [PacketHandler(PacketType.CERTIFY)]
        public static void OnLogin(LoginClient client, INetPacketStream packet)
        {
            var cryptographyService = DependencyContainer.Instance.Resolve<ICryptographyService>();
            var certifyPacket = new CertifyPacket(packet, client.ServerConfiguration.PasswordEncryption);
            string password = null;

            if (client.ServerConfiguration.PasswordEncryption)
            {
                byte[] encryptionKey = cryptographyService.BuildEncryptionKeyFromString(client.ServerConfiguration.EncryptionKey, 16);

                password = cryptographyService.Decrypt(certifyPacket.EncryptedPassword, encryptionKey);
            }
            else
            {
                password = packet.Read<string>();
            }

            if (certifyPacket.BuildVersion != client.ServerConfiguration.BuildVersion)
            {
                Logger.Warn($"Unable to authenticate user from {client.RemoteEndPoint}. Reason: bad client build version.");
                LoginPacketFactory.SendLoginError(client, ErrorType.CERT_GENERAL);
                client.Disconnect();
                return;
            }

            DbUser dbUser = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                dbUser = database.Users.Get(x => x.Username.Equals(certifyPacket.Username, StringComparison.OrdinalIgnoreCase));

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
            
            LoginPacketFactory.SendServerList(client, certifyPacket.Username, client.ClustersConnected);
            client.SetClientUsername(certifyPacket.Username);
            Logger.Info($"User '{client.Username}' logged succesfully from {client.RemoteEndPoint}.");
        }
    }
}
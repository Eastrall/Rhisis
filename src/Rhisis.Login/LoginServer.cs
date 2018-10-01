using Ether.Network.Packets;
using Ether.Network.Server;
using NLog;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Login.ISC;
using Rhisis.Network;
using Rhisis.Network.ISC.Structures;
using Rhisis.Network.Packets;
using System;
using System.Collections.Generic;
using Rhisis.Business;
using Rhisis.Core.DependencyInjection;

namespace Rhisis.Login
{
    public sealed class LoginServer : NetServer<LoginClient>
    {
        private const string LoginConfigFile = "config/login.json";
        private const string DatabaseConfigFile = "config/database.json";

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the ISC server.
        /// </summary>
        public static ISCServer InterServer { get; private set; }

        /// <summary>
        /// Gets the login sever's configuration.
        /// </summary>
        public LoginConfiguration LoginConfiguration { get; private set; }

        /// <summary>
        /// Gets the list of the connected clusters.
        /// </summary>
        public IEnumerable<ClusterServerInfo> ClustersConnected => InterServer?.ClusterServers;

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="LoginServer"/> instance.
        /// </summary>
        public LoginServer()
        {
            this.LoadConfiguration();
        }

        /// <summary>
        /// Loads the login server's configuration.
        /// </summary>
        private void LoadConfiguration()
        {
            Logger.Debug("Loading server configuration from '{0}'...", LoginConfigFile);
            this.LoginConfiguration = ConfigurationHelper.Load<LoginConfiguration>(LoginConfigFile, true);

            this.Configuration.Host = this.LoginConfiguration.Host;
            this.Configuration.Port = this.LoginConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = 1000;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 4096;

            Logger.Trace("Host: {0}, Port: {1}, MaxNumberOfConnections: {2}, Backlog: {3}, BufferSize: {4}",
                this.Configuration.Host,
                this.Configuration.Port,
                this.Configuration.MaximumNumberOfConnections,
                this.Configuration.Backlog,
                this.Configuration.BufferSize);
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            PacketHandler<LoginClient>.Initialize();

            Logger.Debug("Loading database configuration from '{0}'...", DatabaseConfigFile);
            DatabaseFactory.Instance.Initialize(DatabaseConfigFile);
            Logger.Trace($"Database config -> {DatabaseFactory.Instance.Configuration}");

            BusinessLayer.Initialize();
            DependencyContainer.Instance.Initialize().BuildServiceProvider();

            Logger.Info("Starting ISC server...");
            InterServer = new ISCServer(this.LoginConfiguration.ISC);
            InterServer.Start();

            //TODO: Implement this log inside OnStarted method when will be available.
            Logger.Info("Login server is started and listen on {0}:{1}.", this.Configuration.Host, this.Configuration.Port);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(LoginClient client)
        {
            client.Initialize(this);
            Logger.Info("New client connected from {0}.", client.RemoteEndPoint);
            CommonPacketFactory.SendWelcome(client, client.SessionId);
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(LoginClient client)
        {
            Logger.Info("Client disconnected from {0}.", client.RemoteEndPoint);
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            Logger.Error($"Socket error: {exception.Message}");
        }
        
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (InterServer != null)
            {
                InterServer.Stop();
                InterServer.Dispose();
            }

            base.Dispose(disposing);
        }

       
    }
}
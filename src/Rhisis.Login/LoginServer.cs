using Ether.Network.Packets;
using Ether.Network.Server;
using NLog;
using Rhisis.Core.Helpers;
using Rhisis.Core.ISC.Structures;
using Rhisis.Core.Network;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Database.Exceptions;
using Rhisis.Login.ISC;
using System;
using System.Collections.Generic;

namespace Rhisis.Login
{
    public sealed class LoginServer : NetServer<LoginClient>
    {
        private const string LoginConfigFile = "config/login.json";
        private const string DatabaseConfigFile = "config/database.json";
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private ISCServer _interServer;
        
        /// <summary>
        /// Gets the login sever's configuration.
        /// </summary>
        public LoginConfiguration LoginConfiguration { get; private set; }

        /// <summary>
        /// Gets the list of the connected clusters.
        /// </summary>
        public IEnumerable<ClusterServerInfo> ClustersConnected => this._interServer?.Clusters;

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="LoginServer"/> instance.
        /// </summary>
        public LoginServer()
        {
            Console.Title = "Rhisis - Login Server";
            this.LoadConfiguration();
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            PacketHandler<LoginClient>.Initialize();

            var databaseConfiguration = ConfigurationHelper.Load<DatabaseConfiguration>(DatabaseConfigFile, true);

            DatabaseService.Configure(databaseConfiguration);
            if (!DatabaseService.GetContext().DatabaseExists())
                throw new RhisisDatabaseException($"The database '{databaseConfiguration.Database}' doesn't exists yet.");

            this._interServer = new ISCServer(this.LoginConfiguration.ISC);
            this._interServer.Start();

            Logger.Info("Login Server is up.");
        }

        /// <inheritdoc />
        protected override void OnClientConnected(LoginClient connection)
        {
            Logger.Info("New client connected: {0}", connection.Id);
            connection.InitializeClient(this);
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(LoginClient connection)
        {
            Logger.Info("Client {0} disconnected.", connection.Id);
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            Logger.Error(exception.Message);
        }
        
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (this._interServer != null)
            {
                this._interServer.Stop();
                this._interServer.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads the login server's configuration.
        /// </summary>
        private void LoadConfiguration()
        {
            this.LoginConfiguration = ConfigurationHelper.Load<LoginConfiguration>(LoginConfigFile, true);

            this.Configuration.Host = this.LoginConfiguration.Host;
            this.Configuration.Port = this.LoginConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = 1000;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 4096;
        }
    }
}
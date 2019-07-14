﻿using Ether.Network.Packets;
using Ether.Network.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Login.ISC;
using Rhisis.Network;
using Rhisis.Network.ISC.Structures;
using Rhisis.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Login
{
    public sealed class LoginServer : NetServer<LoginClient>, ILoginServer
    {
        private readonly ILogger<LoginServer> _logger;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly ISCConfiguration _iscConfiguration;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the ISC server.
        /// </summary>
        public static ISCServer InterServer { get; private set; }

        /// <summary>
        /// Gets the list of the connected clusters.
        /// </summary>
        public IEnumerable<ClusterServerInfo> ClustersConnected => InterServer?.ClusterServers;

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="LoginServer"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="loginConfiguration">Login server configuration.</param>
        /// <param name="iscConfiguration">ISC configuration.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public LoginServer(ILogger<LoginServer> logger, IOptions<LoginConfiguration> loginConfiguration, IOptions<ISCConfiguration> iscConfiguration, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._loginConfiguration = loginConfiguration.Value;
            this._iscConfiguration = iscConfiguration.Value;
            this._serviceProvider = serviceProvider;
            this.Configuration.Host = this._loginConfiguration.Host;
            this.Configuration.Port = this._loginConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = 1000;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 4096;
            this.Configuration.Blocking = false;

            this._logger.LogTrace("Host: {0}, Port: {1}, MaxNumberOfConnections: {2}, Backlog: {3}, BufferSize: {4}",
                this.Configuration.Host,
                this.Configuration.Port,
                this.Configuration.MaximumNumberOfConnections,
                this.Configuration.Backlog,
                this.Configuration.BufferSize);
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            this._logger.LogInformation("Starting ISC server...");
            InterServer = new ISCServer(this._iscConfiguration);
            InterServer.Start();

            //TODO: Implement this log inside OnStarted method when will be available.
            this._logger.LogInformation("Login server is started and listen on {0}:{1}.", this.Configuration.Host, this.Configuration.Port);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(LoginClient client)
        {
            this._logger.LogInformation($"New client connected from {client.RemoteEndPoint}.");

            client.Initialize(this, this._serviceProvider.GetRequiredService<ILogger<LoginClient>>());
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(LoginClient client)
        {
            if (string.IsNullOrEmpty(client.Username))
                this._logger.LogInformation($"Unknwon client disconnected from {client.RemoteEndPoint}.");
            else
                this._logger.LogInformation($"Client '{client.Username}' disconnected from {client.RemoteEndPoint}.");
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            this._logger.LogInformation($"Socket error: {exception.Message}");
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

        /// <inheritdoc />
        public IEnumerable<ClusterServerInfo> GetConnectedClusters() => InterServer?.ClusterServers;

        /// <inheritdoc />
        public LoginClient GetClientByUsername(string username)
            => this.Clients.FirstOrDefault(x =>
                x.IsConnected &&
                x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        /// <inheritdoc />
        public bool IsClientConnected(string username) => this.GetClientByUsername(username) != null;
    }
}
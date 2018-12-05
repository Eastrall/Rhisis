using Ether.Network.Packets;
using Ether.Network.Server;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.ISC;
using System;
using System.Linq;

namespace Rhisis.World
{
    public sealed partial class WorldServer : NetServer<WorldClient>, IWorldServer
    {
        private readonly ILogger<WorldServer> _logger;
        private readonly WorldConfiguration _worldConfiguration;

        /// <summary>
        /// Gets the ISC client.
        /// </summary>
        public static ISCClient InterClient { get; private set; }

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="WorldServer"/> instance.
        /// </summary>
        public WorldServer(ILogger<WorldServer> logger, WorldConfiguration worldConfiguration)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration;
            this.Configuration.Host = this._worldConfiguration.Host;
            this.Configuration.Port = this._worldConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = 1000;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 4096;

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
            this._logger.LogInformation("Connection to ISC server on {0}:{1}...", this._worldConfiguration.ISC.Host, this._worldConfiguration.ISC.Port);
            InterClient = new ISCClient(this._worldConfiguration);
            InterClient.Connect();

            //TODO: Implement this log inside OnStarted method when will be available.
            this._logger.LogInformation("'{0}' world server is started and listen on {1}:{2}.",
                InterClient.WorldConfiguration.Name, this.Configuration.Host, this.Configuration.Port);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(WorldClient client)
        {
            client.InitializeClient(this);
            this._logger.LogInformation("New client connected from {0}.", client.RemoteEndPoint);
            CommonPacketFactory.SendWelcome(client, client.SessionId);
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(WorldClient client)
        {
            this._logger.LogInformation("Client disconnected from {0}.", client.RemoteEndPoint);
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            this._logger.LogError("WorldServer Error: {0}", exception.Message);
        }

        /// <summary>
        /// Gets a player entity by his id.
        /// </summary>
        /// <param name="id">Player id</param>
        /// <returns></returns>
        public IPlayerEntity GetPlayerEntity(int id)
        {
            WorldClient client =  this.Clients.FirstOrDefault(x => x.Player.Id == id);

            return client?.Player;
        }

        /// <summary>
        /// Gets a player entity by his name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPlayerEntity GetPlayerEntity(string name)
        {
            WorldClient client = this.Clients.FirstOrDefault(x => x.Player.Object.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return client?.Player;
        }
    }
}

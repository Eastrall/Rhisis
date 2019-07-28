using Ether.Network.Packets;
using Ether.Network.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Handlers;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
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
        private const int MaxConnections = 500;
        private const int ClientBufferSize = 128;
        private const int ClientBacklog = 50;

        private readonly ILogger<WorldServer> _logger;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly IGameResources _gameResources;
        private readonly IServiceProvider _serviceProvider;

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="WorldServer"/> instance.
        /// </summary>
        public WorldServer(ILogger<WorldServer> logger, IOptions<WorldConfiguration> worldConfiguration, IGameResources gameResources, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration.Value;
            this.Configuration.Host = this._worldConfiguration.Host;
            this.Configuration.Port = this._worldConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = MaxConnections;
            this.Configuration.Backlog = ClientBacklog;
            this.Configuration.BufferSize = ClientBufferSize;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            this._gameResources.Load(typeof(DefineLoader),
                typeof(TextLoader),
                typeof(MoverLoader),
                typeof(ItemLoader),
                typeof(DialogLoader),
                typeof(ShopLoader),
                typeof(JobLoader),
                typeof(TextClientLoader),
                typeof(ExpTableLoader),
                typeof(PenalityLoader));

            //TODO: Implement this log inside OnStarted method when will be available.
            this._logger.LogInformation("'{0}' world server is started and listen on {1}:{2}.",
                this._worldConfiguration.Name, this.Configuration.Host, this.Configuration.Port);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(WorldClient client)
        {
            this._logger.LogInformation("New client connected from {0}.", client.RemoteEndPoint);

            client.Initialize(this._serviceProvider.GetRequiredService<ILogger<WorldClient>>(),
                this._serviceProvider.GetRequiredService<IHandlerInvoker>());
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
        public IPlayerEntity GetPlayerEntity(uint id)
        {
            WorldClient client = this.Clients.FirstOrDefault(x => x.Player.Id == id);
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

        public IPlayerEntity GetPlayerEntityByCharacterId(uint id)
        {
            WorldClient client = this.Clients.FirstOrDefault(x => x.Player.PlayerData.Id == id);
            return client?.Player;
        }
    }
}

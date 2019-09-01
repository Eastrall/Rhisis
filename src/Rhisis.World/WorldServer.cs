using Ether.Network.Packets;
using Ether.Network.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Client;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Chat;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Sylver.HandlerInvoker;
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
        private readonly IMapManager _mapManager;
        private readonly IBehaviorManager _behaviorManager;
        private readonly IChatCommandManager _chatCommandManager;

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="WorldServer"/> instance.
        /// </summary>
        public WorldServer(ILogger<WorldServer> logger, IOptions<WorldConfiguration> worldConfiguration, IGameResources gameResources, IServiceProvider serviceProvider, IMapManager mapManager, IBehaviorManager behaviorManager, IChatCommandManager chatCommandManager)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration.Value;
            this._gameResources = gameResources;
            this._serviceProvider = serviceProvider;
            this._mapManager = mapManager;
            this._behaviorManager = behaviorManager;
            this._chatCommandManager = chatCommandManager;
            this.Configuration.Host = this._worldConfiguration.Host;
            this.Configuration.Port = this._worldConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = MaxConnections;
            this.Configuration.Backlog = ClientBacklog;
            this.Configuration.BufferSize = ClientBufferSize;
            this.Configuration.Blocking = false;
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
                typeof(ExpTableLoader),
                typeof(PenalityLoader),
                typeof(NpcLoader));

            this._chatCommandManager.Load();
            this._behaviorManager.Load();
            this._mapManager.Load();

            //TODO: Implement this log inside OnStarted method when will be available.
            this._logger.LogInformation("'{0}' world server is started and listen on {1}:{2}.",
                this._worldConfiguration.Name, this.Configuration.Host, this.Configuration.Port);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(WorldClient client)
        {
            client.Initialize(this._serviceProvider.GetRequiredService<ILogger<WorldClient>>(),
                this._serviceProvider.GetRequiredService<IHandlerInvoker>());
            CommonPacketFactory.SendWelcome(client, client.SessionId);

            this._logger.LogInformation("New client connected from {0}.", client.RemoteEndPoint);
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

        /// <inheritdoc />
        public IPlayerEntity GetPlayerEntity(uint id) => this.Clients.FirstOrDefault(x => x.Player.Id == id)?.Player;

        /// <inheritdoc />
        public IPlayerEntity GetPlayerEntity(string name) 
            => this.Clients.FirstOrDefault(x => x.Player.Object.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Player;

        /// <inheritdoc />
        public IPlayerEntity GetPlayerEntityByCharacterId(uint id) 
            => this.Clients.FirstOrDefault(x => x.Player.PlayerData.Id == id)?.Player;
    }
}

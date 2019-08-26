using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Recovery;
using Rhisis.World.Systems.Taskbar;
using System;

namespace Rhisis.World.Game.Factories.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class PlayerFactory : IPlayerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGameResources _gameResources;
        private readonly IMapManager _mapManager;
        private readonly IBehaviorManager _behaviorManager;
        private readonly IInventorySystem _inventorySystem;
        private readonly ITaskbarSystem _taskbarSystem;
        private readonly ObjectFactory _playerFactory;

        /// <summary>
        /// Creates a new <see cref="PlayerFactory"/> instance.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="mapManager">Map manager.</param>
        /// <param name="behaviorManager">Behavior manager.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="taskbarSystem">Taskbar system.</param>
        public PlayerFactory(IServiceProvider serviceProvider, IGameResources gameResources, IMapManager mapManager, IBehaviorManager behaviorManager, IInventorySystem inventorySystem, ITaskbarSystem taskbarSystem)
        {
            this._serviceProvider = serviceProvider;
            this._gameResources = gameResources;
            this._mapManager = mapManager;
            this._behaviorManager = behaviorManager;
            this._inventorySystem = inventorySystem;
            this._taskbarSystem = taskbarSystem;
            this._playerFactory = ActivatorUtilities.CreateFactory(typeof(PlayerEntity), Type.EmptyTypes);
        }

        /// <inheritdoc />
        public IPlayerEntity CreatePlayer(DbCharacter character)
        {
            var player = this._playerFactory(this._serviceProvider, null) as IPlayerEntity;

            IMapInstance map = this._mapManager.GetMap(character.MapId);

            if (map == null)
            {
                throw new InvalidOperationException($"Cannot find map with id '{character.MapId}'.");
            }

            IMapLayer mapLayer = map.GetMapLayer(character.MapLayerId) ?? map.DefaultMapLayer;

            player.Object = new ObjectComponent
            {
                ModelId = character.Gender == 0 ? 11 : 12,
                Type = WorldObjectType.Mover,
                MapId = character.MapId,
                CurrentMap = map,
                LayerId = mapLayer.Id,
                Position = new Vector3(character.PosX, character.PosY, character.PosZ),
                Angle = character.Angle,
                Size = ObjectComponent.DefaultObjectSize,
                Name = character.Name,
                Spawned = false,
                Level = character.Level,
                MovingFlags = ObjectState.OBJSTA_STAND
            };

            player.Health = new HealthComponent
            {
                Hp = character.Hp,
                Mp = character.Mp,
                Fp = character.Fp
            };

            player.VisualAppearance = new VisualAppearenceComponent
            {
                Gender = character.Gender,
                SkinSetId = character.SkinSetId,
                HairId = character.HairId,
                HairColor = character.HairColor,
                FaceId = character.FaceId,
            };

            player.PlayerData = new PlayerDataComponent
            {
                Id = character.Id,
                Slot = character.Slot,
                Gold = character.Gold,
                Authority = (AuthorityType)character.User.Authority,
                Experience = character.Experience,
                JobData = this._gameResources.Jobs[character.ClassId]
            };

            player.Moves = new MovableComponent
            {
                Speed = this._gameResources.Movers[player.Object.ModelId].Speed,
                DestinationPosition = player.Object.Position.Clone(),
                LastMoveTime = Time.GetElapsedTime(),
                NextMoveTime = Time.GetElapsedTime() + 10
            };

            player.Attributes.ResetAttribute(DefineAttributes.STR, character.Strength);
            player.Attributes.ResetAttribute(DefineAttributes.STA, character.Stamina);
            player.Attributes.ResetAttribute(DefineAttributes.DEX, character.Dexterity);
            player.Attributes.ResetAttribute(DefineAttributes.INT, character.Intelligence);

            player.Statistics = new StatisticsComponent(character);
            player.Timers.NextHealTime = Time.TimeInSeconds() + RecoverySystem.NextIdleHealStand;

            player.Behavior = this._behaviorManager.GetDefaultBehavior(BehaviorType.Player, player);

            // Initialize the inventory
            this._inventorySystem.InitializeInventory(player, character.Items);

            // Taskbar
            this._taskbarSystem.InitializeTaskbar(player, character.TaskbarShortcuts);

            mapLayer.AddEntity(player);

            return player;
        }
    }
}

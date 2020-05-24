using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Extensions;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Game;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Entities.Internal;
using Rhisis.World.Game.Maps;
using Rhisis.World.Systems.Recovery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Factories.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class PlayerFactory : IPlayerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRhisisDatabase _database;
        private readonly IGameResources _gameResources;
        private readonly IMapManager _mapManager;
        private readonly IBehaviorManager _behaviorManager;
        private readonly IItemFactory _itemFactory;
        private readonly ObjectFactory _playerFactory;

        /// <summary>
        /// Creates a new <see cref="PlayerFactory"/> instance.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="database">Rhisis database access layer.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="mapManager">Map manager.</param>
        /// <param name="behaviorManager">Behavior manager.</param>
        /// <param name="itemFactory">Item factory.</param>
        public PlayerFactory(IServiceProvider serviceProvider, IRhisisDatabase database, IGameResources gameResources, IMapManager mapManager, IBehaviorManager behaviorManager, IItemFactory itemFactory)
        {
            _serviceProvider = serviceProvider;
            _database = database;
            _gameResources = gameResources;
            _mapManager = mapManager;
            _behaviorManager = behaviorManager;
            _itemFactory = itemFactory;
            _playerFactory = ActivatorUtilities.CreateFactory(typeof(PlayerEntity), Type.EmptyTypes);
        }

        /// <inheritdoc />
        public IPlayerEntity CreatePlayer(DbCharacter character)
        {
            int playerModelId = character.Gender == 0 ? 11 : 12; // TODO: remove these magic numbers

            if (!_gameResources.Movers.TryGetValue(playerModelId, out MoverData moverData))
            {
                throw new ArgumentException($"Cannot find mover with id '{playerModelId}' in game resources.", nameof(playerModelId));
            }

            var player = _playerFactory(_serviceProvider, null) as PlayerEntity;

            IMapInstance map = _mapManager.GetMap(character.MapId);

            if (map == null)
            {
                throw new InvalidOperationException($"Cannot find map with id '{character.MapId}'.");
            }

            IMapLayer mapLayer = map.GetMapLayer(character.MapLayerId) ?? map.DefaultMapLayer;

            player.Object = new ObjectComponent
            {
                ModelId = playerModelId,
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
                Gender = character.Gender.ToString().ToEnum<GenderType>(),
                Slot = character.Slot,
                Gold = character.Gold,
                Authority = (AuthorityType)character.User.Authority,
                Experience = character.Experience,
                JobData = _gameResources.Jobs[(DefineJob.Job)character.JobId]
            };
            player.Moves = new MovableComponent
            {
                Speed = _gameResources.Movers[player.Object.ModelId]?.Speed ?? 0.1f,
                LastMoveTime = Time.GetElapsedTime(),
                NextMoveTime = Time.GetElapsedTime() + 10
            };

            player.Data = moverData;

            player.Attributes[DefineAttributes.HP] = character.Hp;
            player.Attributes[DefineAttributes.MP] = character.Mp;
            player.Attributes[DefineAttributes.FP] = character.Fp;
            player.Attributes[DefineAttributes.STR] = character.Strength;
            player.Attributes[DefineAttributes.STA] = character.Stamina;
            player.Attributes[DefineAttributes.DEX] = character.Dexterity;
            player.Attributes[DefineAttributes.INT] = character.Intelligence;

            player.Statistics = new StatisticsComponent(character);
            player.Timers.NextHealTime = Time.TimeInSeconds() + RecoverySystem.NextIdleHealStand;

            player.Behavior = _behaviorManager.GetDefaultBehavior(BehaviorType.Player, player);
            player.Hand = _itemFactory.CreateInventoryItem(11, 0, ElementType.None, 0);

            var gameServices = _serviceProvider.GetRequiredService<IEnumerable<IGameSystemLifeCycle>>().OrderBy(x => x.Order);

            foreach (IGameSystemLifeCycle service in gameServices)
            {
                service.Initialize(player);
            }

            mapLayer.AddEntity(player);

            return player;
        }

        /// <inheritdoc />
        public void SavePlayer(IPlayerEntity player)
        {
            if (player == null)
                return;

            DbCharacter character = _database.Characters.FirstOrDefault(x => x.Id == player.PlayerData.Id);

            if (character != null)
            {
                character.LastConnectionTime = player.PlayerData.LoggedInAt;
                character.PlayTime += (long)(DateTime.UtcNow - player.PlayerData.LoggedInAt).TotalSeconds;

                character.PosX = player.Object.Position.X;
                character.PosY = player.Object.Position.Y;
                character.PosZ = player.Object.Position.Z;
                character.Angle = player.Object.Angle;
                character.MapId = player.Object.MapId;
                character.MapLayerId = player.Object.LayerId;
                character.Gender = player.VisualAppearance.Gender;
                character.HairColor = player.VisualAppearance.HairColor;
                character.HairId = player.VisualAppearance.HairId;
                character.FaceId = player.VisualAppearance.FaceId;
                character.SkinSetId = player.VisualAppearance.SkinSetId;
                character.Level = player.Object.Level;

                character.JobId = (int)player.PlayerData.Job;
                character.Gold = player.PlayerData.Gold;
                character.Experience = player.PlayerData.Experience;

                character.Strength = player.Attributes[DefineAttributes.STR];
                character.Stamina = player.Attributes[DefineAttributes.STA];
                character.Dexterity = player.Attributes[DefineAttributes.DEX];
                character.Intelligence = player.Attributes[DefineAttributes.INT];
                character.StatPoints = player.Statistics.StatPoints;
                character.SkillPoints = player.Statistics.SkillPoints;

                character.Hp = player.Attributes[DefineAttributes.HP];
                character.Mp = player.Attributes[DefineAttributes.MP];
                character.Fp = player.Attributes[DefineAttributes.FP];

                _database.SaveChanges();

                var gameSystems = _serviceProvider.GetRequiredService<IEnumerable<IGameSystemLifeCycle>>().OrderBy(x => x.Order);

                foreach (IGameSystemLifeCycle system in gameSystems)
                {
                    system.Save(player);
                }
            }
        }
    }
}

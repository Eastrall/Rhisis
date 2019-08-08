using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Common.Game.Structures;
using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Recovery;
using Sylver.HandlerInvoker.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class JoinGameHandler
    {
        private readonly ILogger<JoinGameHandler> _logger;
        private readonly IDatabase _database;
        private readonly IGameResources _gameResources;
        private readonly IMapManager _mapManager;
        private readonly IBehaviorManager _behaviorManager;
        private readonly IInventorySystem _inventorySystem;
        private readonly IWorldSpawnPacketFactory _worldSpawnPacketFactory;

        /// <summary>
        /// Creates a new <see cref="JoinGameHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="database">Database access layer.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="mapManager">Map manager.</param>
        /// <param name="behaviorManager">Behavior manager.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="worldSpawnPacketFactory">World spawn packet factory.</param>
        public JoinGameHandler(ILogger<JoinGameHandler> logger, IDatabase database, IGameResources gameResources, IMapManager mapManager, IBehaviorManager behaviorManager, IInventorySystem inventorySystem, IWorldSpawnPacketFactory worldSpawnPacketFactory)
        {
            this._logger = logger;
            this._database = database;
            this._gameResources = gameResources;
            this._mapManager = mapManager;
            this._behaviorManager = behaviorManager;
            this._inventorySystem = inventorySystem;
            this._worldSpawnPacketFactory = worldSpawnPacketFactory;
        }

        /// <summary>
        /// Prepares the player to join the world.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming join packet.</param>
        [HandlerAction(PacketType.JOIN)]
        public void OnJoin(IWorldClient client, JoinPacket packet)
        {
            DbCharacter character = this._database.Characters.Get(packet.PlayerId);

            if (character == null)
            {
                this._logger.LogError($"Invalid player id received from client; cannot find player with id: {packet.PlayerId}");
                return;
            }

            if (character.IsDeleted)
            {
                this._logger.LogWarning($"Cannot connect with character '{character.Name}' for user '{character.User.Username}'. Reason: character is deleted.");
                return;
            }

            if (character.User.Authority <= 0)
            {
                this._logger.LogWarning($"Cannot connect with '{character.Name}'. Reason: User {character.User.Username} is banned.");
                // TODO: send error to client
                return;
            }

            IMapInstance map = this._mapManager.GetMap(character.MapId);

            if (map == null)
            {
                this._logger.LogWarning($"Map with id '{character.MapId}' doesn't exist.");
                // TODO: send error to client or go to default map ?
                return;
            }

            IMapLayer mapLayer = map.GetMapLayer(character.MapLayerId) ?? map.DefaultMapLayer;

            // 1st: Create the player entity with the map layer context
            client.Player = new PlayerEntity();

            // 2nd: create and initialize the components
            client.Player.Object = new ObjectComponent
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
                Spawned = true,
                Level = character.Level,
                MovingFlags = ObjectState.OBJSTA_STAND
            };

            client.Player.Health = new HealthComponent
            {
                Hp = character.Hp,
                Mp = character.Mp,
                Fp = character.Fp
            };

            client.Player.VisualAppearance = new VisualAppearenceComponent
            {
                Gender = character.Gender,
                SkinSetId = character.SkinSetId,
                HairId = character.HairId,
                HairColor = character.HairColor,
                FaceId = character.FaceId,
            };

            client.Player.PlayerData = new PlayerDataComponent
            {
                Id = character.Id,
                Slot = character.Slot,
                Gold = character.Gold,
                Authority = (AuthorityType)character.User.Authority,
                Experience = character.Experience,
                JobData = this._gameResources.Jobs[character.ClassId]
            };

            client.Player.Moves = new MovableComponent
            {
                Speed = this._gameResources.Movers[client.Player.Object.ModelId].Speed,
                DestinationPosition = client.Player.Object.Position.Clone(),
                LastMoveTime = Time.GetElapsedTime(),
                NextMoveTime = Time.GetElapsedTime() + 10
            };

            client.Player.Attributes.ResetAttribute(DefineAttributes.STR, character.Strength);
            client.Player.Attributes.ResetAttribute(DefineAttributes.STA, character.Stamina);
            client.Player.Attributes.ResetAttribute(DefineAttributes.DEX, character.Dexterity);
            client.Player.Attributes.ResetAttribute(DefineAttributes.INT, character.Intelligence);

            client.Player.Statistics = new StatisticsComponent(character);
            client.Player.Timers.NextHealTime = Time.TimeInSeconds() + RecoverySystem.NextIdleHealStand;

            client.Player.Behavior = this._behaviorManager.GetDefaultBehavior(BehaviorType.Player, client.Player);
            client.Player.Connection = client;

            // Initialize the inventory
            this._inventorySystem.InitializeInventory(client.Player, character.Items);

            // Taskbar
            foreach (var applet in character.TaskbarShortcuts.Where(x => x.TargetTaskbar == ShortcutTaskbarTarget.Applet))
            {
                if (applet.Type == ShortcutType.Item)
                {
                    var item = client.Player.Inventory.GetItem(x => x.Slot == applet.ObjectId);
                    client.Player.Taskbar.Applets.CreateShortcut(new Shortcut(applet.SlotIndex, applet.Type, (uint)item.UniqueId, applet.ObjectType, applet.ObjectIndex, applet.UserId, applet.ObjectData, applet.Text));

                }
                else
                {
                    client.Player.Taskbar.Applets.CreateShortcut(new Shortcut(applet.SlotIndex, applet.Type, applet.ObjectId, applet.ObjectType, applet.ObjectIndex, applet.UserId, applet.ObjectData, applet.Text));
                }
            }

            foreach (var item in character.TaskbarShortcuts.Where(x => x.TargetTaskbar == ShortcutTaskbarTarget.Item))
            {
                if (item.Type == ShortcutType.Item)
                {
                    var inventoryItem = client.Player.Inventory.GetItem(x => x.Slot == item.ObjectId);
                    client.Player.Taskbar.Items.CreateShortcut(new Shortcut(item.SlotIndex, item.Type, (uint)inventoryItem.UniqueId, item.ObjectType, item.ObjectIndex, item.UserId, item.ObjectData, item.Text), item.SlotLevelIndex ?? -1);
                }
                else
                    client.Player.Taskbar.Items.CreateShortcut(new Shortcut(item.SlotIndex, item.Type, item.ObjectId, item.ObjectType, item.ObjectIndex, item.UserId, item.ObjectData, item.Text), item.SlotLevelIndex ?? -1);
            }

            var list = new List<Shortcut>();
            foreach (var skill in character.TaskbarShortcuts.Where(x => x.TargetTaskbar == ShortcutTaskbarTarget.Queue))
            {
                list.Add(new Shortcut(skill.SlotIndex, skill.Type, skill.ObjectId, skill.ObjectType, skill.ObjectIndex, skill.UserId, skill.ObjectData, skill.Text));
            }
            client.Player.Taskbar.Queue.CreateShortcuts(list);

            // 3rd: spawn the player
            this._worldSpawnPacketFactory.SendPlayerSpawn(client.Player);

            // 4th: player is now spawned
            mapLayer.AddEntity(client.Player);
            client.LoggedInAt = DateTime.UtcNow;
        }
    }
}

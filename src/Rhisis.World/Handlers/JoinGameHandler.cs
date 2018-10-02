using Ether.Network.Packets;
using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.Core.Structures;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Inventory.EventArgs;

namespace Rhisis.World.Handlers
{
    public static class JoinGameHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(PacketType.JOIN)]
        public static void OnJoin(WorldClient client, INetPacketStream packet)
        {
            var worldId = packet.Read<int>();
            var playerId = packet.Read<int>();
            var authenticationKey = packet.Read<int>();
            var partyId = packet.Read<int>();
            var guildId = packet.Read<int>();
            var guildWarId = packet.Read<int>();
            var idOfMulti = packet.Read<int>(); // what is this?
            var slot = packet.Read<byte>();
            var playerName = packet.Read<string>();
            var username = packet.Read<string>();
            var password = packet.Read<string>();
            var messengerState = packet.Read<int>();
            var messengerCount = packet.Read<int>();
            DbCharacter character = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                character = database.Characters.Get(playerId);

            if (character == null)
            {
                Logger.Error($"Invalid player id received from client; cannot find player with id: {playerId}");
                return;
            }

            if (character.User.Authority <= 0)
            {
                Logger.Info($"User {character.User.Username} is banned.");
                // TODO: send error to client
                return;
            }

            if (!WorldServer.Maps.TryGetValue(character.MapId, out IMapInstance map))
            {
                Logger.Warn("Map with id '{0}' doesn't exist.", character.MapId);
                // TODO: send error to client or go to default map ?
                return;
            }

            IMapLayer mapLayer = map.GetMapLayer(character.MapLayerId) ?? map.GetDefaultMapLayer();

            // 1st: Create the player entity with the map context
            client.Player = map.CreateEntity<PlayerEntity>();

            // 2nd: create and initialize the components
            client.Player.Object = new ObjectComponent
            {
                ModelId = character.Gender == 0 ? 11 : 12,
                Type = WorldObjectType.Mover,
                MapId = character.MapId,
                LayerId = mapLayer.Id,
                Position = new Vector3(character.PosX, character.PosY, character.PosZ),
                Angle = character.Angle,
                Size = 100,
                Name = character.Name,
                Spawned = false,
                Level = character.Level
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
                Authority = (AuthorityType)character.User.Authority
            };

            client.Player.MovableComponent = new MovableComponent
            {
                Speed = WorldServer.Movers[client.Player.Object.ModelId].Speed,
                DestinationPosition = client.Player.Object.Position.Clone(),
                LastMoveTime = Time.GetElapsedTime(),
                NextMoveTime = Time.GetElapsedTime() + 10
            };

            client.Player.Statistics = new StatisticsComponent(character);
            client.Player.Behavior = WorldServer.PlayerBehaviors.DefaultBehavior;
            client.Player.Connection = client;

            // Initialize the inventory
            var inventoryEventArgs = new InventoryInitializeEventArgs(character.Items);
            client.Player.NotifySystem<InventorySystem>(inventoryEventArgs);

            // 3rd: spawn the player
            WorldPacketFactory.SendPlayerSpawn(client.Player);

            // 4th: player is now spawned
            client.Player.Object.Spawned = true;
        }
    }
}

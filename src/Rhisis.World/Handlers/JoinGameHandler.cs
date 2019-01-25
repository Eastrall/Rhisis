using Ether.Network.Packets;
using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
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
            var joinPacket = new JoinPacket(packet);
            DbCharacter character = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                character = database.Characters.Get(joinPacket.PlayerId);

            if (character == null)
            {
                Logger.Error($"Invalid player id received from client; cannot find player with id: {joinPacket.PlayerId}");
                return;
            }

            if (character.User.Authority <= 0)
            {
                Logger.Info($"User {character.User.Username} is banned.");
                // TODO: send error to client
                return;
            }

            IMapInstance map = DependencyContainer.Instance.Resolve<MapLoader>().GetMapById(character.MapId);

            if (map == null)
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
                Level = character.Level,
                MovingFlags = Core.Data.ObjectState.OBJSTA_STAND
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
                JobId = character.ClassId
            };

            client.Player.MovableComponent = new MovableComponent
            {
                Speed = GameResources.Instance.Movers[client.Player.Object.ModelId].Speed,
                DestinationPosition = client.Player.Object.Position.Clone(),
                LastMoveTime = Time.GetElapsedTime(),
                NextMoveTime = Time.GetElapsedTime() + 10
            };

            client.Player.Statistics = new StatisticsComponent(character);

            var behaviors = DependencyContainer.Instance.Resolve<BehaviorLoader>();
            client.Player.Behavior = behaviors.PlayerBehaviors.DefaultBehavior;
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

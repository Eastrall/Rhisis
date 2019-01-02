using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Chat
{
    public class AddDropCommand
    {
        private static ILogger Logger => DependencyContainer.Instance.Resolve<ILogger<AddDropCommand>>();

        [ChatCommand(".adddrop", AuthorityType.Administrator)]
        [ChatCommand(".ad", AuthorityType.Administrator)]
        public static void OnAddDropCommand(IPlayerEntity player, string[] parameters)
        {
            var drop = player.Object.CurrentLayer.CreateEntity<ItemEntity>();

            drop.Drop.Item = new Structures.Item(23, 1, player.PlayerData.Id);
            drop.Object = new Components.ObjectComponent
            {
                MapId = player.Object.MapId,
                LayerId = player.Object.LayerId,
                ModelId = 23,
                Spawned = true,
                Position = player.Object.Position.Clone(),
                Type = WorldObjectType.Item
            };

            Logger.LogInformation("Drop created");
        }
    }
}
using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Regions;
using System.Linq;

namespace Rhisis.World.Game.Chat
{
    public static class CreateNpcCommand
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [ChatCommand("/createnpc")]
        [ChatCommand("/cn")]
        public static void CreateItem(IPlayerEntity player, string[] parameters)
        {
            Logger.Debug("{0} wants to create an Npc", player.Object.Name);

            if (parameters.Length <= 0)
            {
                Logger.Error("Chat: /createnpc command must have at least one parameter.");
                return;
            }

            if(!int.TryParse(parameters[0], out var moverId))
            {
                MoverData moverData = WorldServer.Movers.Values.FirstOrDefault(x =>
                    string.Equals(x.Name, parameters[0], System.StringComparison.OrdinalIgnoreCase));

                moverId = moverData?.Id ?? -1;
            }

            var amount = 1;

            if (parameters.Length >= 2)
                int.TryParse(parameters[1], out amount);

            var isAggressive = 0;

            if (parameters.Length == 3)
                int.TryParse(parameters[2], out isAggressive);

            // TODO: Add support for aggresiveness
            for(int i = 0; i < amount; i++)
            {
                var monster = player.Context.CreateEntity<MonsterEntity>();

                monster.Object = new ObjectComponent()
                {
                    MapId = player.Object.MapId,
                    LayerId = player.Object.LayerId,
                    ModelId = moverId,
                    Type = WorldObjectType.Mover,
                    Position = player.Object.Position,
                    Name = WorldServer.Movers[moverId].Name,
                    Size = ObjectComponent.DefaultObjectSize,
                    Spawned = true,
                    Level = WorldServer.Movers[moverId].Level
                };

                monster.TimerComponent = new TimerComponent
                {
                    LastMoveTimer = RandomHelper.LongRandom(8, 20)
                };
                monster.Region = new DynamicMoverRegion((int)player.Object.Position.X, (int)player.Object.Position.Z, 20, 20);
                monster.Behavior = WorldServer.MonsterBehaviors.GetBehavior(moverId);
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Dyo;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Factories
{
    [Injectable(ServiceLifetime.Singleton)]
    public class NpcFactory : INpcFactory
    {
        private readonly IGameResources _gameResources;
        private readonly IBehaviorManager behaviorManager;

        public NpcFactory(IGameResources gameResources, IBehaviorManager behaviorManager)
        {
            this._gameResources = gameResources;
            this.behaviorManager = behaviorManager;
        }

        /// <inheritdoc />
        public INpcEntity CreateNpc(IMapContext mapContext, NpcDyoElement element)
        {
            var npc = new NpcEntity
            {
                Object = new ObjectComponent
                {
                    MapId = mapContext.Id,
                    CurrentMap = mapContext as IMapInstance,
                    ModelId = element.Index,
                    Name = element.CharacterKey,
                    Angle = element.Angle,
                    Position = element.Position.Clone(),
                    Size = (short)(ObjectComponent.DefaultObjectSize * element.Scale.X),
                    Spawned = true,
                    Type = WorldObjectType.Mover,
                    Level = 1
                }
            };
            npc.Behavior = behaviorManager.GetBehavior(BehaviorType.Npc, npc, npc.Object.ModelId);
            npc.Timers.LastSpeakTime = RandomHelper.Random(10, 15);

            if (!this._gameResources.Npcs.TryGetValue(npc.Object.Name, out NpcData npcData))
            {
                npc.Data = npcData;
            }

            if (npc.Data != null && npc.Data.HasShop)
            {
                ShopData npcShopData = npc.Data.Shop;
                npc.Shop = new ItemContainerComponent[npcShopData.Items.Length];

                for (var i = 0; i < npcShopData.Items.Length; i++)
                {
                    npc.Shop[i] = new ItemContainerComponent(100);

                    for (var j = 0; j < npcShopData.Items[i].Count && j < npc.Shop[i].MaxCapacity; j++)
                    {
                        ItemBase item = npcShopData.Items[i][j];
                        ItemData itemData = this._gameResources.Items[item.Id];

                        npc.Shop[i].Items[j] = new Item(item.Id, itemData.PackMax, -1, j, j, item.Refine, item.Element, item.ElementRefine);
                    }
                }
            }

            return npc;
        }
    }
}

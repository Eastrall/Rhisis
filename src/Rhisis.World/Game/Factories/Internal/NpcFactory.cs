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

namespace Rhisis.World.Game.Factories.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class NpcFactory : INpcFactory
    {
        private readonly IGameResources _gameResources;
        private readonly IBehaviorManager behaviorManager;
        private readonly IItemFactory _itemFactory;

        /// <summary>
        /// Creates a new <see cref="NpcFactory"/> instance.
        /// </summary>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="behaviorManager">Behavior manager.</param>
        /// <param name="itemFactory">Item Factory.</param>
        public NpcFactory(IGameResources gameResources, IBehaviorManager behaviorManager, IItemFactory itemFactory)
        {
            this._gameResources = gameResources;
            this.behaviorManager = behaviorManager;
            this._itemFactory = itemFactory;
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

            if (this._gameResources.Npcs.TryGetValue(npc.Object.Name, out NpcData npcData))
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
                        ItemDescriptor item = npcShopData.Items[i][j];

                        npc.Shop[i].Items[j] = this._itemFactory.CreateItem(item.Id, item.Refine, item.Element, item.ElementRefine);
                        npc.Shop[i].Items[j].Slot = j;
                        npc.Shop[i].Items[j].UniqueId = j;
                        npc.Shop[i].Items[j].Quantity = npc.Shop[i].Items[j].Data.PackMax;
                    }
                }
            }

            return npc;
        }
    }
}

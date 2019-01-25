using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Dyo;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Game.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rhisis.World.Game.Maps
{
    /// <inheritdoc />
    public class MapInstance : Context, IMapInstance
    {
        private const int DefaultMapLayerId = 1;

        private readonly string _mapPath;
        private readonly List<IMapLayer> _layers;
        private readonly List<IMapRegion> _regions;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private IMapLayer _defaultMapLayer;

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IReadOnlyList<IMapLayer> Layers => this._layers;

        /// <inheritdoc />
        public IReadOnlyList<IMapRegion> Regions => this._regions;

        /// <summary>
        /// Creates a new <see cref="MapInstance"/>.
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="name">Map Name</param>
        /// <param name="mapPath">Map path</param>
        private MapInstance(int id, string name, string mapPath)
        {
            this.Id = id;
            this.Name = name;
            this._mapPath = mapPath;
            this._layers = new List<IMapLayer>();
            this._regions = new List<IMapRegion>();
        }

        /// <inheritdoc />
        public void LoadDyo()
        {
            string dyo = Path.Combine(this._mapPath, this.Name + ".dyo");

            using (var dyoFile = new DyoFile(dyo))
            {
                IEnumerable<NpcDyoElement> npcElements = dyoFile.GetElements<NpcDyoElement>();

                foreach (NpcDyoElement element in npcElements)
                    this.CreateNpc(element);
            }
        }

        /// <inheritdoc />
        public void LoadRgn()
        {
            string rgn = Path.Combine(this._mapPath, this.Name + ".rgn");

            using (var rgnFile = new RgnFile(rgn))
            {
                IEnumerable<IMapRespawnRegion> respawnersRgn = rgnFile.GetElements<RgnRespawn7>()
                    .Select(x => MapRespawnRegion.FromRgnElement(x));

                // TODO: load wrapzones
                // TODO: load collector regions

                this._regions.AddRange(respawnersRgn);
            }
        }

        /// <inheritdoc />
        public IMapLayer CreateMapLayer()
        {
            int id = this.Layers.Max(x => x.Id) + 1;

            return this.CreateMapLayer(id);
        }

        /// <inheritdoc />
        public IMapLayer CreateMapLayer(int id)
        {
            var mapLayer = new MapLayer(this, id);

            this._layers.Add(mapLayer);

            if (this._defaultMapLayer == null)
                this._defaultMapLayer = mapLayer;

            return mapLayer;
        }

        /// <inheritdoc />
        public IMapLayer GetMapLayer(int id) => this._layers.FirstOrDefault(x => x.Id == id);

        /// <inheritdoc />
        public IMapLayer GetDefaultMapLayer() => this._defaultMapLayer;

        /// <inheritdoc />
        public void DeleteMapLayer(int id)
        {
            IMapLayer layer = this.GetMapLayer(id);

            if (layer == null)
                return;

            layer.Dispose();
            this._layers.Remove(layer);
        }

        /// <inheritdoc />
        public override void Update()
        {
            lock (SyncRoot)
            {
                foreach (var entity in this.Entities)
                    SystemManager.Instance.ExecuteUpdatable(entity);

                foreach (var mapLayer in this._layers)
                    mapLayer.Update();
            }
        }

        /// <inheritdoc />
        public void StartUpdateTask()
        {
            Task.Run(async () =>
            {
                const float FrameRatePerSeconds = 60f;
                int previousTime = Environment.TickCount;

                while (true)
                {
                    if (this._cancellationToken.IsCancellationRequested)
                        break;

                    int currentTime = Environment.TickCount;
                    int deltaTime = currentTime - previousTime;
                    previousTime = currentTime;

                    this.GameTime = (deltaTime * FrameRatePerSeconds) / 1000f;

                    try
                    {
                        this.Update();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }

                    await Task.Delay((int)Math.Abs(FrameRatePerSeconds - deltaTime), this._cancellationToken).ConfigureAwait(false);
                }
            }, this._cancellationToken);
        }

        /// <inheritdoc />
        public void StopUpdateTask() => this._cancellationTokenSource.Cancel();

        /// <summary>
        /// Creates a NPC.
        /// </summary>
        /// <param name="element"></param>
        private void CreateNpc(NpcDyoElement element)
        {
            var behaviors = DependencyContainer.Instance.Resolve<BehaviorLoader>();
            var npcs = DependencyContainer.Instance.Resolve<NpcLoader>();
            var npc = this.CreateEntity<NpcEntity>();

            npc.Object = new ObjectComponent
            {
                MapId = this.Id,
                ModelId = element.Index,
                Name = element.Name,
                Angle = element.Angle,
                Position = element.Position.Clone(),
                Size = (short)(ObjectComponent.DefaultObjectSize * element.Scale.X),
                Spawned = true,
                Type = WorldObjectType.Mover,
                Level = 1
            };
            npc.Behavior = behaviors.NpcBehaviors.GetBehavior(npc.Object.ModelId);
            npc.Timers.LastSpeakTime = RandomHelper.Random(10, 15);

            npc.Data = npcs.GetNpcData(npc.Object.Name);


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
                        ItemData itemData = GameResources.Instance.Items[item.Id];

                        npc.Shop[i].Items[j] = new Item(item.Id, itemData.PackMax, -1, j, j, item.Refine, item.Element, item.ElementRefine);
                    }
                }
            }
        }

        /// <summary>
        /// Creates and loads a new map.
        /// </summary>
        /// <param name="mapPath">Map path</param>
        /// <param name="mapName">Map name</param>
        /// <param name="mapId">Map id</param>
        /// <returns></returns>
        public static IMapInstance Create(string mapPath, string mapName, int mapId)
        {
            IMapInstance map = new MapInstance(mapId, mapName, mapPath);

            // TODO: Load map heights, revival zones
            map.LoadDyo();
            map.LoadRgn();
            map.CreateMapLayer(DefaultMapLayerId);

            return map;
        }
    }
}

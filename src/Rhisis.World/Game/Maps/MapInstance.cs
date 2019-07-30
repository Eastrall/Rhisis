using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Dyo;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Rhisis.World.Game.Maps
{
    /// <inheritdoc />
    public class MapInstance : Context, IMapInstance
    {
        private const int DefaultMapLayerId = 1;
        private const int MapLandSize = 128;
        private const int FrameRate = 60;
        private const double UpdateRate = 1000f / FrameRate;

        private readonly string _mapPath;
        private readonly List<IMapLayer> _layers;
        private readonly List<IMapRegion> _regions;
        private readonly System.Timers.Timer _updateTimer;
        private readonly ReaderWriterLockSlim _layerLock;
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger<MapInstance>>();

        private IMapLayer _defaultMapLayer;
        private WldFileInformations _worldInformations;

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IMapRevivalRegion DefaultRevivalRegion { get; private set; }

        /// <inheritdoc />
        public int Width => this._worldInformations.Width;

        /// <inheritdoc />
        public int Length => this._worldInformations.Length;

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
            this._layerLock = new ReaderWriterLockSlim();
            this._updateTimer = new System.Timers.Timer(UpdateRate);
            this._updateTimer.Elapsed += (sender, e) => this.UpdateGameLoop();
        }

        /// <summary>
        /// Loads the world map informations from the WLD file.
        /// </summary>
        private void LoadWld()
        {
            string wldFilePath = Path.Combine(this._mapPath, $"{this.Name}.wld");

            using (var wldFile = new WldFile(wldFilePath))
            {
                this._worldInformations = wldFile.WorldInformations;
            }
        }

        /// <summary>
        /// Load NPC from the DYO file.
        /// </summary>
        private void LoadDyo()
        {
            string dyo = Path.Combine(this._mapPath, $"{this.Name}.dyo");

            using (var dyoFile = new DyoFile(dyo))
            {
                IEnumerable<NpcDyoElement> npcElements = dyoFile.GetElements<NpcDyoElement>();

                foreach (NpcDyoElement element in npcElements)
                    this.CreateNpc(element);
            }
        }

        /// <summary>
        /// Load regions from the RGN file.
        /// </summary>
        private void LoadRgn()
        {
            string rgn = Path.Combine(this._mapPath, $"{this.Name}.rgn");

            using (var rgnFile = new RgnFile(rgn))
            {
                IEnumerable<IMapRespawnRegion> respawnersRgn = rgnFile.GetElements<RgnRespawn7>()
                    .Select(x => MapRespawnRegion.FromRgnElement(x));

                this._regions.AddRange(respawnersRgn);

                foreach (RgnRegion3 region in rgnFile.GetElements<RgnRegion3>())
                {
                    switch (region.Index)
                    {
                        case RegionInfo.RI_REVIVAL:
                            int revivalMapId = this._worldInformations.RevivalMapId == 0 ? this.Id : this._worldInformations.RevivalMapId;
                            var newRevivalRegion = MapRevivalRegion.FromRgnElement(region, revivalMapId);
                            this._regions.Add(newRevivalRegion);
                            break;
                        case RegionInfo.RI_TRIGGER:
                            this._regions.Add(MapTriggerRegion.FromRgnElement(region));
                            break;
                        
                        // TODO: load collector regions
                    }
                }

                if (!this._regions.Any(x => x is IMapRevivalRegion))
                {
                    // Loads the default revival region if no revival region is loaded.
                    this.DefaultRevivalRegion = new MapRevivalRegion(0, 0, 0, 0,
                        this._worldInformations.RevivalMapId, this._worldInformations.RevivalKey, null, false, false);
                }
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

            this._layerLock.EnterWriteLock();
            this._layers.Add(mapLayer);
            this._layerLock.ExitWriteLock();

            if (this._defaultMapLayer == null)
                this._defaultMapLayer = mapLayer;

            return mapLayer;
        }

        /// <inheritdoc />
        public IMapLayer GetMapLayer(int id)
        {
            IMapLayer layer = null;

            this._layerLock.EnterReadLock();
            try
            {
                layer = this._layers.FirstOrDefault(x => x.Id == id);
            }
            finally
            {
                this._layerLock.ExitReadLock();
            }

            return layer;
        }

        /// <inheritdoc />
        public IMapLayer GetDefaultMapLayer() => this._defaultMapLayer;

        /// <inheritdoc />
        public void DeleteMapLayer(int id)
        {
            IMapLayer layer = this.GetMapLayer(id);

            if (layer == null)
                return;

            this._layerLock.EnterWriteLock();

            try
            {
                layer.Dispose();
                this._layers.Remove(layer);
            }
            finally
            {
                this._layerLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            lock (SyncRoot)
            {
                for (int i = 0; i < this.Entities.Count(); i++)
                    SystemManager.Instance.ExecuteUpdatable(this.Entities.ElementAt(i));

                this._layerLock.EnterReadLock();
                try
                {
                    for (int i = 0; i < this._layers.Count; i++)
                        this._layers[i].Update();
                }
                finally
                {
                    this._layerLock.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        public override void UpdateDeletedEntities()
        {
            while (this._entitiesToDelete.TryDequeue(out uint entityIdToDelete))
            {
                var entityToDelete = this.FindEntity<IEntity>(entityIdToDelete);

                if (entityToDelete != null)
                {
                    foreach (IEntity entity in entityToDelete.Object.Entities)
                    {
                        if (entity.Type == WorldEntityType.Player)
                            WorldPacketFactory.SendDespawnObjectTo(entity as IPlayerEntity, entityToDelete);

                        entity.Object.Entities.Remove(entityToDelete);
                    }

                    this._entities.Remove(entityIdToDelete);
                }
            }

            this._layerLock.EnterReadLock();

            try
            {
                for (int i = 0; i < this._layers.Count; i++)
                    this._layers[i].UpdateDeletedEntities();
            }
            finally
            {
                this._layerLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public void StartUpdateTask() => this._updateTimer.Start();

        /// <inheritdoc />
        public void StopUpdateTask() => this._updateTimer.Stop();

        /// <inheritdoc />
        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position) => this.GetNearRevivalRegion(position, false);

        /// <inheritdoc />
        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position, bool isChaoMode)
        {
            IEnumerable<IMapRevivalRegion> revivalRegions = this._regions.Where(x => x is IMapRevivalRegion).Cast<IMapRevivalRegion>();
            var nearestRevivalRegion = revivalRegions.FirstOrDefault(x => x.MapId == this.Id && x.IsChaoRegion == isChaoMode && x.Contains(position) && x.TargetRevivalKey);

            if (nearestRevivalRegion != null)
                return this.GetRevivalRegion(nearestRevivalRegion.Key, isChaoMode);

            revivalRegions = from x in this._regions
                             where x is IMapRevivalRegion y && y.IsChaoRegion == isChaoMode && !y.TargetRevivalKey
                             let region = x as IMapRevivalRegion
                             let distance = position.GetDistance3D(region.RevivalPosition)
                             orderby distance ascending
                             select region;

            return revivalRegions.FirstOrDefault() ?? this.DefaultRevivalRegion;
        }

        /// <inheritdoc />
        public IMapRevivalRegion GetRevivalRegion(string revivalKey) => this.GetRevivalRegion(revivalKey, false);

        /// <inheritdoc />
        public IMapRevivalRegion GetRevivalRegion(string revivalKey, bool isChaoMode)
        {
            IEnumerable<IMapRevivalRegion> revivalRegions = this._regions.Where(x => x is IMapRevivalRegion).Cast<IMapRevivalRegion>();
            IEnumerable<IMapRevivalRegion> revivalRegion = from x in revivalRegions
                                                           where x.Key.Equals(revivalKey, StringComparison.OrdinalIgnoreCase) && x.IsChaoRegion == isChaoMode && !x.TargetRevivalKey
                                                           select x;

            return revivalRegion.FirstOrDefault() ?? this.DefaultRevivalRegion;
        }

        /// <inheritdoc />
        public bool ContainsPosition(Vector3 position)
        {
            float x = position.X / this._worldInformations.MPU;
            float z = position.Z / this._worldInformations.MPU;

            if (x < 0 || x > this.Width * MapLandSize || z < 0 || z > this.Length * MapLandSize)
                return false;

            return true;
        }

        /// <summary>
        /// Updates the Map instance game loop.
        /// </summary>
        private void UpdateGameLoop()
        {
            try
            {
                this.Update();
                this.UpdateDeletedEntities();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"An error occured in map {this.Name}.");
            }
        }

        /// <summary>
        /// Creates a NPC.
        /// </summary>
        /// <param name="element"></param>
        private void CreateNpc(NpcDyoElement element)
        {
            // TODO: move this to factory
            IBehaviorManager behaviorManager = null; //DependencyContainer.Instance.Resolve<BehaviorLoader>();
            IGameResources gameResources = null; //DependencyContainer.Instance.Resolve<NpcLoader>();
            var npc = this.CreateEntity<NpcEntity>();

            npc.Object = new ObjectComponent
            {
                MapId = this.Id,
                ModelId = element.Index,
                Name = element.CharacterKey,
                Angle = element.Angle,
                Position = element.Position.Clone(),
                Size = (short)(ObjectComponent.DefaultObjectSize * element.Scale.X),
                Spawned = true,
                Type = WorldObjectType.Mover,
                Level = 1
            };
            npc.Behavior = behaviorManager.GetBehavior(BehaviorType.Npc, npc, npc.Object.ModelId);
            npc.Timers.LastSpeakTime = RandomHelper.Random(10, 15);

            if (!gameResources.Npcs.TryGetValue(npc.Object.Name, out NpcData npcData))
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
                        ItemData itemData = gameResources.Items[item.Id];

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
            var map = new MapInstance(mapId, mapName, mapPath);

            // TODO: Load map heights
            map.LoadWld();
            map.LoadDyo();
            map.LoadRgn();
            map.CreateMapLayer(DefaultMapLayerId);
            map.StartUpdateTask();

            return map;
        }
    }
}

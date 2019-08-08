using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Maps.Regions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rhisis.World.Game.Maps
{
    public class MapInstance : MapContext, IMapInstance
    {
        private const int DefaultMapLayerId = 1;
        private const int MapLandSize = 128;
        private const int FrameRate = 15;
        private const double UpdateRate = 1000f / FrameRate;

        private readonly ConcurrentDictionary<int, IMapLayer> _layers;
        private readonly IMapFactory _mapFactory;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public WldFileInformations MapInformation { get; }

        /// <inheritdoc />
        public IMapLayer DefaultMapLayer { get; private set; }

        /// <inheritdoc />
        public IMapRevivalRegion DefaultRevivalRegion { get; private set; }

        /// <inheritdoc />
        public int Width => this.MapInformation.Width;

        /// <inheritdoc />
        public int Length => this.MapInformation.Length;

        /// <inheritdoc />
        public IReadOnlyList<IMapLayer> Layers { get; }

        /// <inheritdoc />
        public IReadOnlyList<IMapRegion> Regions { get; private set; }

        /// <summary>
        /// Creates a new <see cref="MapInstance"/>.
        /// </summary>
        /// <param name="id">Map Id.</param>
        /// <param name="name">Map name.</param>
        /// <param name="worldInformations">Map world informations.</param>
        public MapInstance(IMapFactory mapFactory, int id, string name, WldFileInformations worldInformations)
        {
            this.Id = id;
            this._mapFactory = mapFactory;
            this.Name = name;
            this.MapInformation = worldInformations;
            this._layers = new ConcurrentDictionary<int, IMapLayer>();
        }

        // <inheritdoc />
        public IMapLayer CreateMapLayer()
        {
            int layerId = this._layers.Count > 0 ? this._layers.Values.Max(x => x.Id) + 1 : DefaultMapLayerId;

            return this.CreateMapLayer(layerId);
        }

        // <inheritdoc />
        public IMapLayer CreateMapLayer(int id)
        {
            var mapLayer = this._mapFactory.CreateLayer(this, id);

            this._layers.TryAdd(id, mapLayer);

            if (this.DefaultMapLayer == null)
                this.DefaultMapLayer = mapLayer;

            return mapLayer;
        }

        // <inheritdoc />
        public void DeleteMapLayer(int id)
        {
            throw new NotImplementedException();
        }

        // <inheritdoc />
        public IMapLayer GetMapLayer(int id) => this._layers.TryGetValue(id, out IMapLayer layer) ? layer : null;

        // <inheritdoc />
        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position) => this.GetNearRevivalRegion(position, false);

        /// <inheritdoc />
        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position, bool isChaoMode)
        {
            IEnumerable<IMapRevivalRegion> revivalRegions = this.Regions.Where(x => x is IMapRevivalRegion).Cast<IMapRevivalRegion>();
            var nearestRevivalRegion = revivalRegions.FirstOrDefault(x => x.MapId == this.Id && x.IsChaoRegion == isChaoMode && x.Contains(position) && x.TargetRevivalKey);

            if (nearestRevivalRegion != null)
                return this.GetRevivalRegion(nearestRevivalRegion.Key, isChaoMode);

            revivalRegions = from x in this.Regions
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
            IEnumerable<IMapRevivalRegion> revivalRegions = this.Regions.Where(x => x is IMapRevivalRegion).Cast<IMapRevivalRegion>();
            IEnumerable<IMapRevivalRegion> revivalRegion = from x in revivalRegions
                                                           where x.Key.Equals(revivalKey, StringComparison.OrdinalIgnoreCase) && x.IsChaoRegion == isChaoMode && !x.TargetRevivalKey
                                                           select x;

            return revivalRegion.FirstOrDefault() ?? this.DefaultRevivalRegion;
        }

        /// <inheritdoc />
        public bool ContainsPosition(Vector3 position)
        {
            float x = position.X / this.MapInformation.MPU;
            float z = position.Z / this.MapInformation.MPU;

            if (x < 0 || x > this.Width * MapLandSize || z < 0 || z > this.Length * MapLandSize)
                return false;

            return true;
        }

        /// <inheritdoc />
        public void StartUpdateTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var worldEntity in this.Entities)
                        {
                            if (worldEntity.Value is ILivingEntity livingEntity)
                            {
                                livingEntity.Behavior?.Update();
                            }
                        }

                        foreach (var layer in this._layers)
                        {
                            layer.Value.Update();
                        }

                        await Task.Delay((int)UpdateRate);
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            });
        }

        /// <inheritdoc />
        public void StopUpdateTask()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the map regions.
        /// </summary>
        /// <param name="regions">Map regions.</param>
        internal void SetRegions(List<IMapRegion> regions)
        {
            if (!regions.Any(x => x is IMapRevivalRegion))
            {
                // Loads the default revival region if no revival region is loaded.
                this.DefaultRevivalRegion = new MapRevivalRegion(0, 0, 0, 0,
                    this.MapInformation.RevivalMapId, this.MapInformation.RevivalKey, null, false, false);
            }

            this.Regions = regions;
        }

        public override string ToString() => this.Name;
    }
}

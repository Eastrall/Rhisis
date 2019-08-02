using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Maps.Regions;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Maps
{
    public class MapInstance : MapContext, IMapInstance
    {
        private const int DefaultMapLayerId = 1;
        private const int MapLandSize = 128;
        private const int FrameRate = 60;
        private const double UpdateRate = 1000f / FrameRate;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public WldFileInformations MapInformation { get; }

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

        public MapInstance(int id, string name, WldFileInformations worldInformations)
        {
            this.Id = id;
            this.Name = name;
            this.MapInformation = worldInformations;
        }

        public bool ContainsPosition(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public IMapLayer CreateMapLayer()
        {
            throw new System.NotImplementedException();
        }

        public IMapLayer CreateMapLayer(int id)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteMapLayer(int id)
        {
            throw new System.NotImplementedException();
        }

        public IMapLayer GetDefaultMapLayer()
        {
            throw new System.NotImplementedException();
        }

        public IMapLayer GetMapLayer(int id)
        {
            throw new System.NotImplementedException();
        }

        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public IMapRevivalRegion GetNearRevivalRegion(Vector3 position, bool isChaoMode)
        {
            throw new System.NotImplementedException();
        }

        public IMapRevivalRegion GetRevivalRegion(string revivalKey)
        {
            throw new System.NotImplementedException();
        }

        public IMapRevivalRegion GetRevivalRegion(string revivalKey, bool isChaoMode)
        {
            throw new System.NotImplementedException();
        }

        public void StartUpdateTask()
        {
            throw new System.NotImplementedException();
        }

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
    }
}

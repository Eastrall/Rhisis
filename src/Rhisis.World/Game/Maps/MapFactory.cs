using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Dyo;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Maps.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.World.Game.Maps
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class MapFactory : IMapFactory
    {
        private readonly ILogger<MapFactory> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly INpcFactory _npcFactory;

        /// <summary>
        /// Creates a new <see cref="MapFactory"/> instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="npcFactory"></param>
        public MapFactory(ILogger<MapFactory> logger, IServiceProvider serviceProvider, INpcFactory npcFactory)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._npcFactory = npcFactory;
        }

        /// <inheritdoc />
        public IMapInstance Create(string mapPath, string mapName, int mapId)
        {
            WldFileInformations worldInformations = this.LoadWld(mapPath, mapName);

            var mapInstance = ActivatorUtilities.CreateInstance<MapInstance>(this._serviceProvider, mapId, mapName, worldInformations);

            this.LoadDyo(mapPath, mapInstance);
            this.LoadRgn(mapPath, mapInstance);

            return mapInstance;
        }

        /// <inheritdoc />
        public IMapLayer CreateLayer(IMapInstance parentMapInstance, int layerId)
        {
            var mapLayer = ActivatorUtilities.CreateInstance<MapLayer>(this._serviceProvider, parentMapInstance, layerId);

            return mapLayer;
        }

        /// <summary>
        /// Loads the world map informations from the WLD file.
        /// </summary>
        private WldFileInformations LoadWld(string mapPath, string mapName)
        {
            string wldFilePath = Path.Combine(mapPath, $"{mapName}.wld");

            using (var wldFile = new WldFile(wldFilePath))
            {
                return wldFile.WorldInformations;
            }
        }

        /// <summary>
        /// Load NPC from the DYO file.
        /// </summary>
        private void LoadDyo(string mapPath, IMapInstance map)
        {
            string dyo = Path.Combine(mapPath, $"{map.Name}.dyo");

            using (var dyoFile = new DyoFile(dyo))
            {
                IEnumerable<NpcDyoElement> npcElements = dyoFile.GetElements<NpcDyoElement>();

                foreach (NpcDyoElement element in npcElements)
                {
                    map.AddEntity(this._npcFactory.CreateNpc(map, element));
                }
            }
        }

        /// <summary>
        /// Load regions from the RGN file.
        /// </summary>
        private void LoadRgn(string mapPath, MapInstance map)
        {
            var regions = new List<IMapRegion>();
            string rgn = Path.Combine(mapPath, $"{map.Name}.rgn");

            using (var rgnFile = new RgnFile(rgn))
            {
                IEnumerable<IMapRespawnRegion> respawnersRgn = rgnFile.GetElements<RgnRespawn7>()
                    .Select(x => MapRespawnRegion.FromRgnElement(x));

                regions.AddRange(respawnersRgn);

                foreach (RgnRegion3 region in rgnFile.GetElements<RgnRegion3>())
                {
                    switch (region.Index)
                    {
                        case RegionInfo.RI_REVIVAL:
                            int revivalMapId = map.MapInformation.RevivalMapId == 0 ? map.Id : map.MapInformation.RevivalMapId;
                            var newRevivalRegion = MapRevivalRegion.FromRgnElement(region, revivalMapId);
                            regions.Add(newRevivalRegion);
                            break;
                        case RegionInfo.RI_TRIGGER:
                            regions.Add(MapTriggerRegion.FromRgnElement(region));
                            break;

                            // TODO: load collector regions
                    }
                }
            }

            map.SetRegions(regions);
        }
    }
}

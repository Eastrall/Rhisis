using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Configuration;
using Rhisis.World.Game.Factories;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Rhisis.World.Game.Maps
{
    [Injectable(ServiceLifetime.Singleton)]
    public class MapManager : IMapManager
    {
        private readonly ILogger<MapManager> _logger;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly IMemoryCache _cache;
        private readonly IMapFactory _mapFactory;
        private readonly IDictionary<int, IMapInstance> _maps;
        private readonly IDictionary<string, int> _defines;

        public MapManager(ILogger<MapManager> logger, IOptions<WorldConfiguration> worldConfiguration, IMemoryCache cache, IMapFactory mapFactory)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration.Value;
            this._cache = cache;
            this._mapFactory = mapFactory;
            this._defines = this._cache.Get<IDictionary<string, int>>(GameResourcesConstants.Defines);
            this._maps = new ConcurrentDictionary<int, IMapInstance>();
        }

        /// <inheritdoc />
        public IMapInstance GetMap(int id) => this._maps.TryGetValue(id, out IMapInstance map) ? map : null;

        /// <inheritdoc />
        public void Load()
        {
            string worldScriptPath = GameResourcesConstants.Paths.WorldScriptPath;
            var worldsPaths = new Dictionary<string, string>();

            using (var textFile = new TextFile(worldScriptPath))
            {
                foreach (var text in textFile.Texts)
                    worldsPaths.Add(text.Key, text.Value.Replace('"', ' ').Trim());
            }

            foreach (string mapDefineName in this._worldConfiguration.Maps)
            {
                if (!worldsPaths.TryGetValue(mapDefineName, out string mapName))
                {
                    this._logger.LogWarning(GameResourcesConstants.Errors.UnableLoadMapMessage, mapDefineName, $"map is not declared inside '{worldScriptPath}' file");
                    continue;
                }

                if (!this._defines.TryGetValue(mapDefineName, out int mapId))
                {
                    this._logger.LogWarning(GameResourcesConstants.Errors.UnableLoadMapMessage, mapDefineName, $"map has no define id inside '{GameResourcesConstants.Paths.DataSub0Path}/defineWorld.h' file");
                    continue;
                }

                if (_maps.ContainsKey(mapId))
                {
                    this._logger.LogWarning(GameResourcesConstants.Errors.UnableLoadMapMessage, mapDefineName, $"another map with id '{mapId}' already exist.");
                    continue;
                }

                IMapInstance map = this._mapFactory.Create(Path.Combine(GameResourcesConstants.Paths.MapsPath, mapName), mapName, mapId); 

                _maps.Add(mapId, map);
            }

            this._logger.LogInformation("-> {0} maps loaded.", _maps.Count);
        }
    }
}

using Microsoft.Extensions.Logging;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.World.Game.Maps;
using System.Collections.Generic;
using System.IO;

namespace Rhisis.World.Game.Loaders
{
    public sealed class MapLoader : IGameResourceLoader
    {
        private readonly ILogger<MapLoader> _logger;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly DefineLoader _defines;
        private readonly IDictionary<int, IMapInstance> _maps;

        /// <summary>
        /// Gets the map by his id.
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <returns></returns>
        public IMapInstance this[int id] => this.GetMapById(id);

        /// <summary>
        /// Creates a new <see cref="MapLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="worldConfiguration">World Server configuration</param>
        /// <param name="defines">Defines loader</param>
        public MapLoader(ILogger<MapLoader> logger, WorldConfiguration worldConfiguration, DefineLoader defines)
        {
            this._logger = logger;
            this._worldConfiguration = worldConfiguration;
            this._defines = defines;
            this._maps = new Dictionary<int, IMapInstance>();
        }

        /// <inheritdoc />
        public void Load()
        {
            var worldsPaths = new Dictionary<string, string>();
            using (var textFile = new TextFile(GameResources.WorldScriptPath))
            {
                foreach (var text in textFile.Texts)
                    worldsPaths.Add(text.Key, text.Value.Replace('"', ' ').Trim());
            }

            foreach (string mapDefineName in this._worldConfiguration.Maps)
            {
                if (!worldsPaths.TryGetValue(mapDefineName, out string mapName))
                {
                    this._logger.LogWarning(GameResources.UnableLoadMapMessage, mapDefineName, $"map is not declared inside '{GameResources.WorldScriptPath}' file");
                    continue;
                }

                if (!this._defines.Defines.TryGetValue(mapDefineName, out int mapId))
                {
                    this._logger.LogWarning(GameResources.UnableLoadMapMessage, mapDefineName, $"map has no define id inside '{GameResources.DataSub0Path}/defineWorld.h' file");
                    continue;
                }

                if (_maps.ContainsKey(mapId))
                {
                    this._logger.LogWarning(GameResources.UnableLoadMapMessage, mapDefineName, $"another map with id '{mapId}' already exist.");
                    continue;
                }

                IMapInstance map = MapInstance.Create(Path.Combine(GameResources.MapsPath, mapName), mapName, mapId);

                _maps.Add(mapId, map);
                map.StartUpdateTask();
            }

            this._logger.LogInformation("-> {0} maps loaded.", _maps.Count);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // TODO
        }

        /// <summary>
        /// Gets a map by id.
        /// </summary>
        /// <param name="id">Map id</param>
        /// <returns>Map if exists, null otherwise</returns>
        public IMapInstance GetMapById(int id) => this._maps.TryGetValue(id, out IMapInstance value) ? value : null;
    }
}

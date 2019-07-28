using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Game;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    /// <summary>
    /// Loads all server's penality data.
    /// </summary>
    public sealed class PenalityLoader : IGameResourceLoader
    {
        private readonly ILogger<PenalityLoader> _logger;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Creates a new <see cref="PenalityLoader"/> instance.
        /// </summary>
        /// <param name="logger"></param>
        public PenalityLoader(ILogger<PenalityLoader> logger, IMemoryCache cache)
        {
            this._logger = logger;
            this._cache = cache;
        }

        /// <inheritdoc />
        public void Load()
        {
            string deathPenalityPath = GameResourcesConstants.Paths.DeathPenalityPath;

            if (!File.Exists(deathPenalityPath))
            {
                this._logger.LogWarning("Unable to load death penality. Reason: cannot find '{0}' file.", deathPenalityPath);
                return;
            }

            var penalityData = ConfigurationHelper.Load<DeathPenalityData>(deathPenalityPath);

            if (penalityData == null)
            {
                this._logger.LogError(GameResourcesConstants.Errors.UnableLoadMessage, "death penality", "Json loading error.");
                return;
            }

            penalityData.RevivalPenality = penalityData.RevivalPenality.OrderBy(x => x.Level);
            penalityData.DecExpPenality = penalityData.DecExpPenality.OrderBy(x => x.Level);
            penalityData.LevelDownPenality = penalityData.LevelDownPenality.OrderBy(x => x.Level);

            this._cache.Set(GameResourcesConstants.PenalityData, penalityData);
            this._logger.LogInformation("-> Penalities loaded.");
        }
    }
}

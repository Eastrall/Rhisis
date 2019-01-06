using Microsoft.Extensions.Logging;
using Rhisis.Core.Extensions;
using Rhisis.Core.Resources.Include;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    public class ExpTableLoader : IGameResourceLoader
    {
        private readonly ILogger<ExpTableLoader> _logger;

        /// <summary>
        /// Gets the item drop luck data.
        /// </summary>
        public IEnumerable<long[]> ExpDropLuck { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ExpTableLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        public ExpTableLoader(ILogger<ExpTableLoader> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc />
        public void Load()
        {
            if (!File.Exists(GameResources.ExpTablePath))
            {
                this._logger.LogWarning("Unable to load exp table. Reason: cannot find '{0}' file.", GameResources.ExpTablePath);
                return;
            }

            using (var expTableFile = new IncludeFile(GameResources.ExpTablePath, @"([(){}=,;\n\r\t ])"))
            {
                this.LoadDropLuck(expTableFile.GetBlock("expDropLuck"));
            }

            this._logger.LogInformation("-> Experience tables loaded.");
        }

        /// <summary>
        /// Load a collection of item drop luck from a "expDropLuck" block instruction.
        /// </summary>
        /// <param name="dropLuckBlock"></param>
        private void LoadDropLuck(Block dropLuckBlock) 
            => this.ExpDropLuck = dropLuckBlock.UnknownStatements.Select(x => long.Parse(x)).GroupBy(11).Select(x => x.ToArray());

        /// <summary>
        /// Gets a drop luck by level and refine.
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="refine">Refine</param>
        /// <returns></returns>
        public long GetDropLuck(int level, int refine) => this.ExpDropLuck.ElementAt(level - 1).ElementAt(refine);

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose yet.
        }
    }
}

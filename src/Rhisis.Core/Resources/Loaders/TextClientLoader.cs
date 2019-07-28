using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Resources.Include;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class TextClientLoader : IGameResourceLoader
    {
        private readonly ILogger<TextClientLoader> _logger;
        private readonly IMemoryCache _cache;
        private readonly IDictionary<string, string> _texts;

        /// <summary>
        /// Gets the client text associated to the client text id
        /// </summary>
        /// <param name="clientTextId">Client text id</param>
        /// <returns>if client text id exists; null otherwise</returns>
        public string this[string clientTextId] => null;

        /// <summary>
        /// Creates a new <see cref="TextClientLoader" /> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="cache">Memory cache.</param>
        public TextClientLoader(ILogger<TextClientLoader> logger, IMemoryCache cache)
        {
            this._logger = logger;
            this._cache = cache;
            this._texts = this._cache.Get<IDictionary<string, string>>(GameResourcesConstants.Texts);
        }

        /// <inheritdoc />
        public void Load()
        {
            string textClientPath = GameResourcesConstants.Paths.TextClientPath;

            if (!File.Exists(textClientPath))
            {
                this._logger.LogWarning($"Unable to load client texts. Reason: cannot find '{textClientPath}' file.");
                return;
            }

            var textClientData = new ConcurrentDictionary<string, string>();

            using (var textClientFile = new IncludeFile(textClientPath, @"([(){}=,;\n\r])"))
            {
                foreach (var textClientStatement in textClientFile.Statements)
                {
                    if (!(textClientStatement is Block textClientBlock))
                        continue;

                    var regex = new Regex(@"[a-zA-Z0-9_]+").Match(textClientBlock.Name);
                    textClientData.TryAdd(regex.Value, this._texts[textClientBlock.UnknownStatements.ElementAt(0)]);
                }
            }

            this._cache.Set(GameResourcesConstants.ClientTexts, textClientData);
            this._logger.LogInformation($"-> {textClientData.Count} client texts loaded.");
        }
    }
}

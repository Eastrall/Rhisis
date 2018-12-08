using Microsoft.Extensions.Logging;
using Rhisis.Core.Resources.Include;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class TextClientLoader : IGameResourceLoader
    {
        private readonly ILogger<TextClientLoader> _logger;
        private readonly TextLoader _textsLoader;
        private readonly IDictionary<string, string> _textClientData;

        /// <summary>
        /// Gets the client text associated to the client text id
        /// </summary>
        /// <param name="clientTextId">Client text id</param>
        /// <returns>if client text id exists; null otherwise</returns>
        public string this[string clientTextId] => GetClientText(clientTextId);

        /// <summary>
        /// Creates a new <see cref="TextClientLoader" instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="texts">Text loader</param>
        public TextClientLoader(ILogger<TextClientLoader> logger, TextLoader texts)
        {
            this._logger = logger;
            this._textsLoader = texts;
            this._textClientData = new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public void Load()
        {
            if (!File.Exists(GameResources.TextClientPath))
            {
                this._logger.LogWarning($"Unable to load client texts. Reason: cannot find '{GameResources.TextClientPath}' file.");
                return;
            }

            using (var textClientFile = new IncludeFile(GameResources.TextClientPath, @"([(){}=,;\n\r])"))
            {
                foreach (var textClientStatement in textClientFile.Statements)
                {
                    if (!(textClientStatement is Block textClientBlock))
                        continue;

                    var regex = new Regex(@"[a-zA-Z0-9_]+").Match(textClientBlock.Name);
                    _textClientData.Add(regex.Value, _textsLoader.Texts[textClientBlock.UnknownStatements.ElementAt(0)]);
                }
            }

            this._logger.LogInformation($"-> {_textClientData.Count} client texts loaded.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._textClientData.Clear();
        }

        /// <summary>
        /// Gets a client text by the client text id.
        /// </summary>
        /// <param name="clientTextId">Client text id</param>
        /// <returns></returns>
        public string GetClientText(string clientTextId) => this._textClientData.TryGetValue(clientTextId, out string value) ? value : string.Empty;
    }
}

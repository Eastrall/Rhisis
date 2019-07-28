using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rhisis.Core.Structures.Game;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class ShopLoader : IGameResourceLoader
    {
        private readonly ILogger<ShopLoader> _logger;
        private readonly IMemoryCache _cache;
        private readonly JsonLoadSettings _jsonSettings = new JsonLoadSettings { CommentHandling = CommentHandling.Ignore };

        /// <summary>
        /// Creates a new <see cref="ShopLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="cache">Memory cache.</param>
        public ShopLoader(ILogger<ShopLoader> logger, IMemoryCache cache)
        {
            this._logger = logger;
            this._cache = cache;
        }

        /// <inheritdoc />
        public void Load()
        {
            string shopPath = GameResourcesConstants.Paths.ShopsPath;

            if (!Directory.Exists(shopPath))
            {
                this._logger.LogWarning("Unable to load shops. Reason: cannot find '{0}' directory.", shopPath);
                return;
            }

            string[] shopsFiles = Directory.GetFiles(shopPath);
            var shopData = new ConcurrentDictionary<string, ShopData>();

            foreach (string shopFile in shopsFiles)
            {
                string shopFileContent = File.ReadAllText(shopFile);
                var shopsParsed = JToken.Parse(shopFileContent, this._jsonSettings);

                if (shopsParsed.Type == JTokenType.Array)
                {
                    var shops = shopsParsed.ToObject<ShopData[]>();

                    foreach (ShopData shop in shops)
                        this.AddShop(shopData, shop);
                }
                else
                {
                    this.AddShop(shopData, shopsParsed.ToObject<ShopData>());
                }
            }

            this._cache.Set(GameResourcesConstants.Shops, shopData);
            this._logger.LogInformation("-> {0} shops loaded.", shopData.Count);
        }

        /// <summary>
        /// Adds a new shop.
        /// </summary>
        /// <param name="shop">Shop to add</param>
        private void AddShop(IDictionary<string, ShopData> shopData, ShopData shop)
        {
            if (shopData.ContainsKey(shop.Name))
                this._logger.LogWarning(GameResourcesConstants.Errors.ObjectIgnoredMessage, "Shop", shop.Name, "already declared");
            else
                shopData.Add(shop.Name, shop);
        }
    }
}

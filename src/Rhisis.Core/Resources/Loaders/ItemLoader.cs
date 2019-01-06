using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class ItemLoader : IGameResourceLoader
    {
        private readonly ILogger<ItemLoader> _logger;
        private readonly DefineLoader _defines;
        private readonly TextLoader _texts;
        private readonly IDictionary<int, ItemData> _itemsData;

        /// <summary>
        /// Gets an <see cref="ItemData"/> by his id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemData this[int id] => this.GetItem(id);

        /// <summary>
        /// Gets an <see cref="ItemData"/> by his name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ItemData this[string name] => this._itemsData.Values.FirstOrDefault(x => string.Equals(x.Name, name, System.StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Creates a new <see cref="ItemLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="defines">Defines</param>
        /// <param name="texts">Texts</param>
        public ItemLoader(ILogger<ItemLoader> logger, DefineLoader defines, TextLoader texts)
        {
            this._logger = logger;
            this._defines = defines;
            this._texts = texts;
            this._itemsData = new Dictionary<int, ItemData>();
        }

        /// <inheritdoc />
        public void Load()
        {
            if (!File.Exists(GameResources.ItemsPropPath))
            {
                this._logger.LogWarning("Unable to load items. Reason: cannot find '{0}' file.", GameResources.ItemsPropPath);
                return;
            }

            using (var propItem = new ResourceTableFile(GameResources.ItemsPropPath, 1, this._defines.Defines, this._texts.Texts))
            {
                var items = propItem.GetRecords<ItemData>();

                foreach (var item in items)
                {
                    if (this._itemsData.ContainsKey(item.Id))
                    {
                        this._itemsData[item.Id] = item;
                        this._logger.LogWarning(GameResources.ObjectOverridedMessage, "Item", item.Id, "already declared");
                    }
                    else
                        this._itemsData.Add(item.Id, item);
                }
            }

            this._logger.LogInformation("-> {0} items loaded.", this._itemsData.Count);
        }

        /// <summary>
        /// Gets an <see cref="ItemData"/> by his id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemData GetItem(int id) => this._itemsData.TryGetValue(id, out ItemData value) ? value : null;

        /// <summary>
        /// Gets items matching a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<ItemData> GetItems(Func<ItemData, bool> predicate) => this._itemsData.Values.Where(predicate);

        /// <inheritdoc />
        public void Dispose()
        {
            this._itemsData.Clear();
        }
    }
}

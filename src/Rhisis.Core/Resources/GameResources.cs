using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.Core.Resources.Loaders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rhisis.Core.Resources
{
    public class GameResources : Singleton<GameResources>
    {
        public static readonly string DataPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
        public static readonly string DialogsPath = Path.Combine(DataPath, "dialogs");
        public static readonly string ResourcePath = Path.Combine(DataPath, "res");
        public static readonly string MapsPath = Path.Combine(DataPath, "maps");
        public static readonly string ShopsPath = Path.Combine(DataPath, "shops");
        public static readonly string DataSub0Path = Path.Combine(ResourcePath, "data");
        public static readonly string DataSub1Path = Path.Combine(ResourcePath, "dataSub1");
        public static readonly string DataSub2Path = Path.Combine(ResourcePath, "dataSub2");
        public static readonly string MoversPropPath = Path.Combine(DataSub0Path, "propMover.txt");
        public static readonly string ItemsPropPath = Path.Combine(DataSub2Path, "propItem.txt");
        public static readonly string WorldScriptPath = Path.Combine(DataSub0Path, "World.inc");
        public static readonly string JobPropPath = Path.Combine(DataSub1Path, "propJob.inc");
        public static readonly string TextClientPath = Path.Combine(DataSub1Path, "textClient.inc");

        // Logs messages
        public const string UnableLoadMapMessage = "Unable to load map '{0}'. Reason: {1}.";
        public const string ObjectIgnoredMessage = "{0} id '{1}' was ignored. Reason: {2}.";
        public const string ObjectOverridedMessage = "{0} id '{1}' was overrided. Reason: {2}.";

        private ILogger<GameResources> _logger;
        private IEnumerable<Type> _loaders;
        private MoverLoader _movers;
        private ItemLoader _items;

        /// <summary>
        /// Gets the movers data.
        /// </summary>
        public MoverLoader Movers => this._movers ?? (this._movers = DependencyContainer.Instance.Resolve<MoverLoader>());

        /// <summary>
        /// Gets the items data.
        /// </summary>
        public ItemLoader Items => this._items ?? (this._items = DependencyContainer.Instance.Resolve<ItemLoader>());

        /// <summary>
        /// Initialize the <see cref="GameResources"/> with loaders.
        /// </summary>
        /// <param name="loaderTypes"></param>
        public void Initialize(IEnumerable<Type> loaderTypes)
        {
            this._loaders = loaderTypes;

            foreach (var loaderType in this._loaders)
                DependencyContainer.Instance.Register(loaderType, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Load resources.
        /// </summary>
        public void Load()
        {
            this._logger = this._logger ?? DependencyContainer.Instance.Resolve<ILogger<GameResources>>();
            this._logger.LogInformation("Loading resources...");

            Profiler.Start("LoadResources");

            foreach (var loaderType in this._loaders)
            {
                var loader = DependencyContainer.Instance.Resolve(loaderType) as IGameResourceLoader;

                try
                {
                    loader.Load();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"An error occured with loader {loader.GetType().Name}.");
                }
            }

            this._logger.LogInformation("Resources loaded in {0}ms.", Profiler.Stop("LoadResources").ElapsedMilliseconds);
        }

        /// <summary>
        /// Dispose a loader.
        /// </summary>
        /// <typeparam name="TLoader"></typeparam>
        public void DisposeLoader<TLoader>() where TLoader : class, IGameResourceLoader
        {
            var loader = DependencyContainer.Instance.Resolve<TLoader>();

            loader?.Dispose();
        }
    }
}

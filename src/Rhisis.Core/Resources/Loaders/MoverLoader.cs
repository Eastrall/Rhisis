using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.Resources.Include;
using Rhisis.Core.Structures.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class MoverLoader : IGameResourceLoader
    {
        private readonly ILogger<MoverLoader> _logger;
        private readonly IMemoryCache _cache;
        private readonly IDictionary<string, int> _defines;
        private readonly IDictionary<string, string> _texts;

        /// <summary>
        /// Creates a new <see cref="MoverLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="defines">Defines data</param>
        /// <param name="texts">Texts data</param>
        public MoverLoader(ILogger<MoverLoader> logger, IMemoryCache cache)
        {
            this._logger = logger;
            this._cache = cache;
            this._defines = this._cache.Get<IDictionary<string, int>>(GameResourcesConstants.Defines);
            this._texts = this._cache.Get<IDictionary<string, string>>(GameResourcesConstants.Texts);
        }

        /// <inheritdoc />
        public void Load()
        {
            string propMoverPath = GameResourcesConstants.Paths.MoversPropPath;
            string propMoverExPath = GameResourcesConstants.Paths.MoversPropExPath;

            if (!File.Exists(propMoverPath))
            {
                this._logger.LogWarning("Unable to load movers. Reason: cannot find '{0}' file.", propMoverPath);
                return;
            }

            if (!File.Exists(propMoverExPath))
            {
                this._logger.LogWarning("Unable to load movers extras. Reason: cannot find '{0}' file.", propMoverExPath);
                return;
            }

            var moversData = new ConcurrentDictionary<int, MoverData>();

            using (var moversPropFile = new ResourceTableFile(propMoverPath, 1, this._defines, this._texts))
            {
                var movers = moversPropFile.GetRecords<MoverData>();

                foreach (var mover in movers)
                {
                    if (moversData.ContainsKey(mover.Id))
                    {
                        moversData[mover.Id] = mover;
                        this._logger.LogWarning(GameResourcesConstants.Errors.ObjectOverridedMessage, "Mover", mover.Id, "already declared");
                    }
                    else
                        moversData.TryAdd(mover.Id, mover);
                }
            }

            using (var moversPropExFile = new IncludeFile(propMoverExPath))
            {
                foreach (Block moverBlock in moversPropExFile.Statements)
                {
                    if (this._defines.TryGetValue(moverBlock.Name, out int moverId) && moversData.TryGetValue(moverId, out MoverData mover))
                    {
                        this.LoadDropGold(mover, moverBlock.GetInstruction("DropGold"));
                        this.LoadDropItems(mover, moverBlock.GetInstructions("DropItem"));
                        this.LoadDropItemsKind(mover, moverBlock.GetInstructions("DropKind"));
                        
                        mover.MaxDropItem = int.Parse(moverBlock.GetVariable("Maxitem").Value.ToString());
                    }
                }
            }

            this._cache.Set(GameResourcesConstants.Movers, moversData);
            this._logger.LogInformation($"-> {moversData.Count} movers loaded.");
        }

        /// <summary>
        /// Loads the DropGold instruction for a given mover.
        /// </summary>
        /// <param name="mover">Mover</param>
        /// <param name="dropGoldInstruction">DropGold instruction</param>
        private void LoadDropGold(MoverData mover, Instruction dropGoldInstruction)
        {
            if (dropGoldInstruction == null)
                return;

            if (dropGoldInstruction.Parameters.Count < 2)
            {
                this._logger.LogWarning($"Cannot load 'DropGold' instruction for mover {mover.Name}. Reason: Missing parameters.");
                return;
            }

            if (!int.TryParse(dropGoldInstruction.Parameters.ElementAt(0).ToString(), out int minGold))
            {
                this._logger.LogWarning($"Cannot load min gold amount for mover {mover.Name}.");
            }

            if (!int.TryParse(dropGoldInstruction.Parameters.ElementAt(1).ToString(), out int maxGold))
            {
                this._logger.LogWarning($"Cannot load max gold amount for mover {mover.Name}.");
            }

            mover.DropGoldMin = minGold;
            mover.DropGoldMax = maxGold;
        }

        /// <summary>
        /// Loads a collection of DropItem instruction for a given mover.
        /// </summary>
        /// <param name="mover">Mover</param>
        /// <param name="dropItemInstructions">Collection of DropItem instructions</param>
        private void LoadDropItems(MoverData mover, IEnumerable<Instruction> dropItemInstructions)
        {
            if (dropItemInstructions == null)
                return;

            foreach (var dropItemInstruction in dropItemInstructions)
            {
                var dropItem = new DropItemData();

                string dropItemName = dropItemInstruction.Parameters.ElementAt(0).ToString();
                if (this._defines.TryGetValue(dropItemName, out int itemId))
                    dropItem.ItemId = itemId;
                else
                {
                    this._logger.LogWarning($"Cannot find drop item id: {dropItemName} for mover {mover.Name}.");
                    continue;
                }

                if (long.TryParse(dropItemInstruction.Parameters.ElementAt(1).ToString(), out long probability))
                    dropItem.Probability = probability;
                else
                {
                    this._logger.LogWarning($"Cannot read drop item probability for item {dropItemName} and mover {mover.Name}.");
                }

                if (int.TryParse(dropItemInstruction.Parameters.ElementAt(2).ToString(), out int itemMaxRefine))
                    dropItem.ItemMaxRefine = itemMaxRefine;
                else
                    this._logger.LogWarning($"Cannot read drop item refine max for item {dropItemName} and mover {mover.Name}.");

                if (int.TryParse(dropItemInstruction.Parameters.ElementAt(3).ToString(), out int itemCount))
                    dropItem.Count = itemCount;
                else
                    this._logger.LogWarning($"Cannot read drop item count for item {dropItemName} and mover {mover.Name}.");

                mover.DropItems.Add(dropItem);
            }
        }

        /// <summary>
        /// Loads a collection of DropKind instructions for a given mover.
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="instructions"></param>
        private void LoadDropItemsKind(MoverData mover, IEnumerable<Instruction> instructions)
        {
            if (instructions == null)
                return;

            foreach (var dropItemKindInstruction in instructions)
            {
                var dropItemKind = new DropItemKindData();

                if (dropItemKindInstruction.Parameters.Count < 0 || dropItemKindInstruction.Parameters.Count > 3)
                {
                    this._logger.LogWarning($"Cannot load 'DropKind' instruction for mover {mover.Name}. Reason: Missing parameters.");
                    continue;
                }

                string itemKind = dropItemKindInstruction.Parameters.ElementAt(0).ToString().Replace("IK3_", string.Empty);
                dropItemKind.ItemKind = (ItemKind3)Enum.Parse(typeof(ItemKind3), itemKind);

                // From official files: Project.cpp:2824
                dropItemKind.UniqueMin = mover.Level - 5;
                dropItemKind.UniqueMax = mover.Level - 2;

                if (dropItemKind.UniqueMin < 1)
                    dropItemKind.UniqueMin = 1;
                if (dropItemKind.UniqueMax < 1)
                    dropItemKind.UniqueMax = 1;

                mover.DropItemsKind.Add(dropItemKind);
            }
        }
    }
}

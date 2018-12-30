using Microsoft.Extensions.Logging;
using Rhisis.Core.Resources.Include;
using Rhisis.Core.Structures.Game;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class MoverLoader : IGameResourceLoader
    {
        private readonly ILogger<MoverLoader> _logger;
        private readonly DefineLoader _definesLoader;
        private readonly TextLoader _textsLoader;
        private readonly IDictionary<int, MoverData> _moversData;

        /// <summary>
        /// Gets a <see cref="MoverData"/> by his mover id.
        /// </summary>
        /// <param name="moverId"></param>
        /// <returns></returns>
        public MoverData this[int moverId] => this.GetMover(moverId);

        /// <summary>
        /// Creates a new <see cref="MoverLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="defines">Defines data</param>
        /// <param name="texts">Texts data</param>
        public MoverLoader(ILogger<MoverLoader> logger, DefineLoader defines, TextLoader texts)
        {
            this._logger = logger;
            this._definesLoader = defines;
            this._textsLoader = texts;
            this._moversData = new Dictionary<int, MoverData>();
        }

        /// <inheritdoc />
        public void Load()
        {
            if (!File.Exists(GameResources.MoversPropPath))
            {
                this._logger.LogWarning("Unable to load movers. Reason: cannot find '{0}' file.", GameResources.MoversPropPath);
                return;
            }

            using (var moversPropFile = new ResourceTableFile(GameResources.MoversPropPath, 1, this._definesLoader.Defines, this._textsLoader.Texts))
            {
                var movers = moversPropFile.GetRecords<MoverData>();

                foreach (var mover in movers)
                {
                    if (this._moversData.ContainsKey(mover.Id))
                    {
                        this._moversData[mover.Id] = mover;
                        this._logger.LogWarning(GameResources.ObjectOverridedMessage, "Mover", mover.Id, "already declared");
                    }
                    else
                        this._moversData.Add(mover.Id, mover);
                }
            }

            using (var moversPropExFile = new IncludeFile(GameResources.MoversPropExPath))
            {
                foreach (Block moverBlock in moversPropExFile.Statements)
                {
                    if (this._definesLoader.Defines.TryGetValue(moverBlock.Name, out int moverId) && 
                        this._moversData.TryGetValue(moverId, out MoverData mover))
                    {
                        this.LoadDropGold(mover, moverBlock.GetInstruction("DropGold"));
                        this.LoadDropItems(mover, moverBlock.GetInstructions("DropItem"));
                    }
                }
            }

            this._logger.LogInformation("-> {0} movers loaded.", this._moversData.Count);
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
                if (this._definesLoader.Defines.TryGetValue(dropItemName, out int itemId))
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
        /// Gets a <see cref="MoverData"/> by his mover id.
        /// </summary>
        /// <param name="moverId"></param>
        /// <returns></returns>
        public MoverData GetMover(int moverId) => this._moversData.TryGetValue(moverId, out MoverData value) ? value : null;

        /// <inheritdoc />
        public void Dispose()
        {
            this._moversData.Clear();
        }
    }
}

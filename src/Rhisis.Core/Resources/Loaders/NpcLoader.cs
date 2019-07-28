using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Resources.Include;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Game;
using Rhisis.Core.Structures.Game.Dialogs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Loaders
{
    public sealed class NpcLoader : IGameResourceLoader
    {
        private readonly ILogger<NpcLoader> _logger;
        private readonly IMemoryCache _cache;
        private readonly IGameResources _gameResources;
        private readonly WorldConfiguration _configuration;
        private readonly IDictionary<string, string> _texts;

        /// <summary>
        /// Creates a new <see cref="NpcLoader"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="cache">Memory cache.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="worldConfiguration">World configuration.</param>
        public NpcLoader(ILogger<NpcLoader> logger, IMemoryCache cache, IGameResources gameResources, IOptions<WorldConfiguration> worldConfiguration)
        {
            this._logger = logger;
            this._cache = cache;
            this._gameResources = gameResources;
            this._configuration = worldConfiguration.Value;
            this._texts = this._cache.Get<IDictionary<string, string>>(GameResourcesConstants.Texts);
        }

        /// <inheritdoc />
        public void Load()
        {
            IEnumerable<string> files = Directory.GetFiles(GameResourcesConstants.Paths.ResourcePath, "character*.inc", SearchOption.AllDirectories);

            var npcData = new ConcurrentDictionary<string, NpcData>();

            foreach (var file in files)
            {
                using (var npcFile = new IncludeFile(file))
                {
                    foreach (IStatement npcStatement in npcFile.Statements)
                    {
                        if (!(npcStatement is Block npcBlock))
                            continue;

                        var npcId = npcStatement.Name;
                        var npcName = npcId;

                        // We gets the npc name.
                        foreach (IStatement npcInfoStatement in npcBlock.Statements)
                        {
                            if (npcInfoStatement is Instruction instruction && npcInfoStatement.Name == "SetName")
                            {
                                if (instruction.Parameters.Count > 0)
                                {
                                    npcName = this._texts[instruction.Parameters.First().ToString()];
                                }
                            }
                        }
                        //TODO: implement other npc settings (image, music, actions...)
                        //      + constants for statement (like SetName)

                        // We gets shop and dialog of this npc.
                        this._gameResources.Shops.TryGetValue(npcId, out ShopData shop);

                        DialogData dialogData = null;
                        if (this._gameResources.Dialogs.TryGetValue(this._configuration.Language, out DialogSet dialogSet))
                        {
                            dialogSet.TryGetValue(npcId, out dialogData);
                        }

                        var npc = new NpcData(npcId, npcName, shop, dialogData);

                        if (npcData.ContainsKey(npc.Id))
                        {
                            npcData[npc.Id] = npc;
                            this._logger.LogWarning(GameResourcesConstants.Errors.ObjectOverridedMessage, "NPC", npc.Id, "already declared");
                        }
                        else
                        {
                            npcData.TryAdd(npc.Id, npc);
                        }
                    }
                }
            }

            this._cache.Set(GameResourcesConstants.Npcs, npcData);
            this._logger.LogInformation($"-> {npcData.Count} NPCs loaded.");
        }
    }
}

using Rhisis.World.Game.Core.Systems;
using System.Collections.Generic;

namespace Rhisis.World.Systems.PlayerData.EventArgs
{
    public class QueryPlayerData2EventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the amount of entries.
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// Gets the player id and version.
        /// </summary>
        public Dictionary<uint, int> PlayerDictionary { get; }

        public QueryPlayerData2EventArgs(uint size, Dictionary<uint, int> playerDictionary)
        {
            this.Size = size;
            this.PlayerDictionary = playerDictionary;
        }

        public override bool CheckArguments()
        {
            return Size <= 1024 && Size > 0;
        }
    }
}

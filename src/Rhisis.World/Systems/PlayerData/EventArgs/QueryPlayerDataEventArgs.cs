using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.PlayerData.EventArgs
{
    public class QueryPlayerDataEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the player id.
        /// </summary>
        public uint PlayerId { get; }

        /// <summary>
        /// Gets the player data version.
        /// </summary>
        public int Version { get; }

        public QueryPlayerDataEventArgs(uint playerId, int version)
        {
            this.PlayerId = playerId;
            this.Version = version;
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}

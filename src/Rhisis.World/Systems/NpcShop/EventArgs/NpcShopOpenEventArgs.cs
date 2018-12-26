using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.NpcShop.EventArgs
{
    internal sealed class NpcShopOpenEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the NPC Object id.
        /// </summary>
        public uint NpcObjectId { get; }

        /// <summary>
        /// Creates a new <see cref="NpcShopOpenEventArgs"/> instance.
        /// </summary>
        /// <param name="npcObjectId"></param>
        public NpcShopOpenEventArgs(uint npcObjectId)
        {
            this.NpcObjectId = npcObjectId;
        }

        /// <inheritdoc />
        public override bool CheckArguments() => this.NpcObjectId > 0;
    }
}

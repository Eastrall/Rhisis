using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.NpcDialog
{
    public class NpcDialogOpenEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the NPC object id.
        /// </summary>
        public uint NpcObjectId { get; }

        /// <summary>
        /// Gets the dialog key.
        /// </summary>
        public string DialogKey { get; }

        /// <inheritdoc />
        public NpcDialogOpenEventArgs(uint objectId, string dialogKey)
        {
            this.NpcObjectId = objectId;
            this.DialogKey = dialogKey;
        }

        /// <inheritdoc />
        public override bool CheckArguments() => this.NpcObjectId > 0;
    }
}

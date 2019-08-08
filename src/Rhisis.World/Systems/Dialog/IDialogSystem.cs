using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Dialog
{
    public interface IDialogSystem
    {
        /// <summary>
        /// Opens a NPC dialog at a given dialog key.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="npcObjectId">NPC object id.</param>
        /// <param name="dialogKey">Dialog key.</param>
        void OpenNpcDialog(IPlayerEntity player, uint npcObjectId, string dialogKey);
    }
}

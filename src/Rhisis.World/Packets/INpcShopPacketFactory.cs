using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface INpcShopPacketFactory
    {
        /// <summary>
        /// Sends the NPC shop to the player.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="npc">NPC</param>
        void SendOpenNpcShop(IPlayerEntity player, INpcEntity npc);
    }
}

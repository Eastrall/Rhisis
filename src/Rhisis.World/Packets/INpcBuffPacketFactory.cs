using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface INpcBuffPacketFactory
    {
        /// <summary>
        /// Sends a NPC buff packet to the player.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="buff">Buff</param>
        void NpcGivesBuff(IPlayerEntity player, string buff);
    }
}


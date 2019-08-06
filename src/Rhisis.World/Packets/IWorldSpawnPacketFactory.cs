using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IWorldSpawnPacketFactory
    {
        /// <summary>
        /// Send the spawn packet to the current player.
        /// </summary>
        /// <param name="player">Current player</param>
        void SendPlayerSpawn(IPlayerEntity player);

        /// <summary>
        /// Sends the spawn object to the current player.
        /// </summary>
        /// <param name="player">Current player</param>
        /// <param name="entityToSpawn">Entity to spawn</param>
        void SendSpawnObjectTo(IPlayerEntity player, IWorldEntity entityToSpawn);

        /// <summary>
        /// Sends the despawn object to the current player.
        /// </summary>
        /// <param name="player">Current player</param>
        /// <param name="entityToDespawn">Entity to despawn</param>
        void SendDespawnObjectTo(IPlayerEntity player, IWorldEntity entityToDespawn);
    }
}

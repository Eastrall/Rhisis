using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IPlayerDataPacketFactory
    {
        /// <summary>
        /// Write data of a single player. Can contain data of multiplate players when filled with send = false.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        /// <param name="jobId"></param>
        /// <param name="level"></param>
        /// <param name="gender"></param>
        /// <param name="online"></param>
        /// <param name="send">Decides if the packet gets send to the player</param>
        void SendPlayerData(IPlayerEntity entity, uint playerId, string name, byte jobId, byte level, byte gender, int version, bool online, bool send = true);

        /// <summary>
        /// Sends a packet that modifies the player mode.
        /// </summary>
        /// <param name="entity"></param>
        void SendModifyMode(IPlayerEntity entity);
    }
}

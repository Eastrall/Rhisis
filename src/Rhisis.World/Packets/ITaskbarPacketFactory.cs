using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface ITaskbarPacketFactory
    {
        /// <summary>
        /// Sends the player action points.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="actionPoint">Action points.</param>
        void SendSetActionPoint(IPlayerEntity player, int actionPoint);
    }
}
using Ether.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface IPacketFactoryUtilities
    {
        /// <summary>
        /// Sends the packet to all visible entities of the specified entity.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="entity">Entity.</param>
        /// <param name="sendToPlayer">Send packet to player if entity is a player.</param>
        void SendToVisible(INetPacketStream packet, IWorldEntity entity, bool sendToPlayer = false);
    }
}

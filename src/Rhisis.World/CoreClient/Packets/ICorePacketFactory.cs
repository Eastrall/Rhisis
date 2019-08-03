using Rhisis.Core.Structures.Configuration;
using Rhisis.Network.Core;

namespace Rhisis.World.CoreClient.Packets
{
    /// <summary>
    /// Provides methods to send packets to the core server.
    /// </summary>
    public interface ICorePacketFactory
    {
        /// <summary>
        /// Sends an authentication request to the core server.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="worldConfiguration">World server configuration.</param>
        void SendAuthentication(ICoreClient client, WorldConfiguration worldConfiguration);
    }
}

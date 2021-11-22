﻿using Rhisis.ClusterServer.Abstractions;
using Rhisis.ClusterServer.Structures;
using Rhisis.Network;
using System.Collections.Generic;

namespace Rhisis.ClusterServer.Packets
{
    /// <summary>
    /// Provides a factory to communicate with a cluster client.
    /// </summary>
    public interface IClusterPacketFactory
    {
        /// <summary>
        /// Sends a welcome packet to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        void SendWelcome(IClusterUser client);
        
        /// <summary>
        /// Sends a pong packet in response of a ping packet.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="time">Ping time.</param>
        void SendPong(IClusterUser client, int time);

        /// <summary>
        /// Sends a cluster server error to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="errorType">Cluster server error type.</param>
        void SendClusterError(IClusterUser client, ErrorType errorType);

        /// <summary>
        /// Sends the client's characters list.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="authenticationKey">Client authentication key.</param>
        /// <param name="characters">A list of the client's available characters.</param>
        void SendPlayerList(IClusterUser client, int authenticationKey, IEnumerable<ClusterCharacter> characters);

        /// <summary>
        /// Sends the selected world server host address.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="address">Selected world server host address.</param>
        void SendWorldAddress(IClusterUser client, string address);

        /// <summary>
        /// Sends the login num pad to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="loginProtectValue">Login protect value.</param>
        void SendLoginNumPad(IClusterUser client, int loginProtectValue);

        /// <summary>
        /// Sends the login protect value.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="loginProtectValue">Login protect value.</param>
        void SendLoginProtect(IClusterUser client, int loginProtectValue);

        /// <summary>
        /// Sends the join world to the client.
        /// </summary>
        /// <param name="client">Client.</param>
        void SendJoinWorld(IClusterUser client);

        /// <summary>
        /// Sends a query to retrieve the client's tick count.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="time">Time.</param>
        void SendQueryTickCount(IClusterUser client, uint time);
    }
}

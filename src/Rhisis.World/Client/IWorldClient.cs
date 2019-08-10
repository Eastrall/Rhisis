using Ether.Network.Common;
using Rhisis.World.Game.Entities;
using System;

namespace Rhisis.World.Client
{
    public interface IWorldClient : INetUser
    {
        /// <summary>
        /// Gets the ID assigned to this session.
        /// </summary>
        uint SessionId { get; }

        /// <summary>
        /// Gets or sets the player entity.
        /// </summary>
        IPlayerEntity Player { get; set; }

        /// <summary>
        /// Gets the remote end point (IP and port) for this client.
        /// </summary>
        string RemoteEndPoint { get; }
    }
}

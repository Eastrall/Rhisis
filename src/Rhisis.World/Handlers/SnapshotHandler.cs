using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Network.Packets;
using Rhisis.World.Client;
using Sylver.HandlerInvoker;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class SnapshotHandler
    {
        private readonly ILogger<SnapshotHandler> _logger;
        private readonly IHandlerInvoker _handlerInvoker;

        /// <summary>
        /// Creates a new <see cref="SnapshotHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="handlerInvoker">Handler invoker.</param>
        public SnapshotHandler(ILogger<SnapshotHandler> logger, IHandlerInvoker handlerInvoker)
        {
            this._logger = logger;
            this._handlerInvoker = handlerInvoker;
        }

        /// <summary>
        /// Receives and handles a snapshot.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet with snapshots.</param>
        [HandlerAction(PacketType.SNAPSHOT)]
        public void OnSnapshot(IWorldClient client, INetPacketStream packet)
        {
            var snapshotCount = packet.Read<byte>();

            while (snapshotCount > 0)
            {
                var snapshotHeaderNumber = packet.Read<short>();

                try
                {
                    var snapshotHeader = (SnapshotType)snapshotHeaderNumber;

                    this._handlerInvoker.Invoke(snapshotHeader, client, packet);

                }
                catch (ArgumentNullException)
                {
                    if (Enum.IsDefined(typeof(SnapshotType), snapshotHeaderNumber))
                        this._logger.LogWarning("Received an unimplemented World snapshot {0} (0x{1}) from {2}.", Enum.GetName(typeof(SnapshotType), snapshotHeaderNumber), snapshotHeaderNumber.ToString("X4"), client.RemoteEndPoint);
                    else
                        this._logger.LogWarning("[SECURITY] Received an unknown World snapshot 0x{0} from {1}.", snapshotHeaderNumber.ToString("X4"), client.RemoteEndPoint);
                }
                catch (Exception exception)
                {
                    this._logger.LogError(exception, $"An error occured while handling a world snapshot.");
                    this._logger.LogDebug(exception.InnerException?.StackTrace);
                }

                snapshotCount--;
            }
        }
    }
}

using Ether.Network.Packets;
using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Packets
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class PacketFactoryUtilities : IPacketFactoryUtilities
    {
        /// <inheritdoc />
        public void SendToVisible(INetPacketStream packet, IWorldEntity entity, bool sendToPlayer = false)
        {
            IEnumerable<IPlayerEntity> visiblePlayers = from x in entity.Object.Entities
                                                        where x.Type == WorldEntityType.Player
                                                        select x as IPlayerEntity;

            foreach (IPlayerEntity visiblePlayer in visiblePlayers)
                visiblePlayer.Connection.Send(packet);

            if (sendToPlayer && entity is IPlayerEntity player)
                player.Connection.Send(packet);
        }
    }
}

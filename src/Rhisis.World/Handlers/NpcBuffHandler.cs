using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.NpcBuff;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    [Handler]
    public sealed class NpcBuffHandler
    {
        private readonly INpcBuffSystem _npcBuffSystem;

        /// <summary>
        /// Creates a new <see cref="NpcBuffHandler"/>.
        /// </summary>
        /// <param name="npcBuffSystem"></param>
        public NpcBuffHandler(INpcBuffSystem npcBuffSystem)
        {
            this._npcBuffSystem = npcBuffSystem;
        }

        /// <summary>
        /// Player requests buff from an assist NPC.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.NPC_BUFF)]
        public void OnNpcBuffMenu(IWorldClient client, NpcBuffPacket packet)
        {
            this._npcBuffSystem.NpcBuff(client.Player, packet.buff);
        }
    }
}

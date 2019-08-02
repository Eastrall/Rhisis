using Rhisis.Core.Data;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Sends a special effect to every entities around the given entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="specialEffect">Special effect Id.</param>
        public static void SendSpecialEffect(IWorldEntity entity, int specialEffectId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.CREATESFXOBJ);
                packet.Write(specialEffectId);
                packet.Write(0f); // X
                packet.Write(0f); // Y
                packet.Write(0f); // Z
                packet.Write(false); // Flag

                SendToVisible(packet, entity, true);
            }
        }

        /// <summary>
        /// Sends a special effect to every entities around the given entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="specialEffect">Special effect.</param>
        public static void SendSpecialEffect(IWorldEntity entity, DefineSpecialEffects specialEffect) 
            => SendSpecialEffect(entity, (int)specialEffect);
    }
}

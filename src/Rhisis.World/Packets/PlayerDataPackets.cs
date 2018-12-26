using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using System;
using System.Runtime.InteropServices;

namespace Rhisis.World.Packets
{

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerData
    {
        public sbyte JobId;
        public sbyte Level;
        public sbyte Gender;
        public int Version;
        public sbyte Online;

        public byte[] ToByteArray()
        {
            var size = Marshal.SizeOf(this);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }

    public static partial class WorldPacketFactory
    {
        static int ReverseBytes(int val)
        {
            return (val & 0x000000FF) << 24 |
                    (val & 0x0000FF00) << 8 |
                    (val & 0x00FF0000) >> 8 |
                    ((int)(val & 0xFF000000)) >> 24;
        }

        static sbyte ReverseBytes(sbyte val)
        {
            return (sbyte)((val & 0x00FF) << 8);
        }


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
        public static void SendPlayerData(IPlayerEntity entity, uint playerId, string name, sbyte jobId, sbyte level, sbyte gender, bool online, bool send = true)
        {
            var s = new PlayerData
            {
                JobId = jobId,
                Level = level,
                Gender = gender,
                Version = 2,
                Online = Convert.ToSByte(online)
            };

            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(FFPacket.NullId, SnapshotType.QUERY_PLAYER_DATA);

                packet.Write(playerId);
                packet.Write(name);

                packet.Write(s.ToByteArray());

                if (send) 
                    entity.Connection.Send(packet);
            }
        }
    }
}

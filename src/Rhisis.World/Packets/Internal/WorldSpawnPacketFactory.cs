using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Systems.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class WorldSpawnPacketFactory : IWorldSpawnPacketFactory
    {
        /// <inheritdoc />
        public void SendPlayerSpawn(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.ENVIRONMENTALL, 0x0000FF00);
                packet.Write(0); // Get weather by season

                packet.StartNewMergedPacket(player.Id, SnapshotType.WORLD_READINFO);
                packet.Write(player.Object.MapId);
                packet.Write(player.Object.Position.X);
                packet.Write(player.Object.Position.Y);
                packet.Write(player.Object.Position.Z);

                packet.StartNewMergedPacket(player.Id, SnapshotType.ADD_OBJ);

                // Common object properties
                packet.Write((byte)player.Object.Type);
                packet.Write(player.Object.ModelId);
                packet.Write((byte)player.Object.Type);
                packet.Write(player.Object.ModelId);
                packet.Write(player.Object.Size);
                packet.Write(player.Object.Position.X);
                packet.Write(player.Object.Position.Y);
                packet.Write(player.Object.Position.Z);
                packet.Write((short)(player.Object.Angle * 10));
                packet.Write(player.Id);

                packet.Write<short>(0); // m_dwMotion
                packet.Write<byte>(1); // m_bPlayer
                packet.Write(player.Health.Hp); // HP
                packet.Write((int)player.Object.MovingFlags); // moving flags
                packet.Write((int)player.Object.MotionFlags); // motion flags
                packet.Write<byte>(1); // m_dwBelligerence

                packet.Write(-1); // m_dwMoverSfxId

                packet.Write(player.Object.Name);
                packet.Write(player.VisualAppearance.Gender);
                packet.Write((byte)player.VisualAppearance.SkinSetId);
                packet.Write((byte)player.VisualAppearance.HairId);
                packet.Write(player.VisualAppearance.HairColor);
                packet.Write((byte)player.VisualAppearance.FaceId);
                packet.Write(player.PlayerData.Id);
                packet.Write((byte)player.PlayerData.JobId); // Job

                packet.Write((short)player.Attributes[DefineAttributes.STR]);
                packet.Write((short)player.Attributes[DefineAttributes.STA]);
                packet.Write((short)player.Attributes[DefineAttributes.DEX]);
                packet.Write((short)player.Attributes[DefineAttributes.INT]);

                packet.Write((short)player.Object.Level); // Level
                packet.Write(-1); // Fuel
                packet.Write(0); // Actuel fuel

                // Guilds

                packet.Write<byte>(0); // have guild or not
                packet.Write(0); // guild cloak

                // Party

                packet.Write<byte>(0); // have party or not

                packet.Write((byte)player.PlayerData.Authority); // authority
                packet.Write((uint)player.PlayerData.Mode); // mode
                packet.Write(0); // state mode
                packet.Write(0x00000000); // item used ??
                packet.Write(0); // last pk time.
                packet.Write(0); // karma
                packet.Write(0); // pk propensity
                packet.Write(0); // pk exp
                packet.Write(0); // fame
                packet.Write<byte>(0); // duel
                packet.Write(-1); // titles

                // Serialize visible effects
                IEnumerable<Item> equippedItems = player.Inventory.Items.GetRange(InventorySystem.EquipOffset, InventorySystem.MaxItems - InventorySystem.EquipOffset);

                foreach (var item in equippedItems)
                    packet.Write(item.Refines);

                packet.Write(0); // guild war state

                for (var i = 0; i < 26; ++i)
                    packet.Write(0);

                packet.Write((short)player.Health.Mp); // MP
                packet.Write((short)player.Health.Fp); // FP
                packet.Write(0); // tutorial state
                packet.Write(0); // fly experience
                packet.Write(player.PlayerData.Gold); // Gold
                packet.Write(player.PlayerData.Experience); // exp
                packet.Write(0); // skill level
                packet.Write((int)player.Statistics.SkillPoints); // skill points
                packet.Write<long>(0); // death exp
                packet.Write(0); // death level

                for (var i = 0; i < 32; ++i)
                    packet.Write(0); // job in each level

                packet.Write(0); // marking world id
                packet.Write(player.Object.Position.X);
                packet.Write(player.Object.Position.Y);
                packet.Write(player.Object.Position.Z);

                // Quests
                packet.Write<byte>(0);
                packet.Write<byte>(0);
                packet.Write<byte>(0);

                packet.Write(0); // murderer id
                packet.Write((short)player.Statistics.StatPoints); // stat points
                packet.Write<short>(0); // always 0

                // item mask
                foreach (var item in equippedItems)
                    packet.Write(item.Id);

                // skills
                for (var i = 0; i < 45; ++i)
                {
                    packet.Write(-1); // skill id
                    packet.Write(0); // skill level
                }

                packet.Write<byte>(0); // cheer point
                packet.Write(0); // next cheer point ?

                // Bank
                packet.Write((byte)player.PlayerData.Slot);
                for (var i = 0; i < 3; ++i)
                    packet.Write(0); // gold
                for (var i = 0; i < 3; ++i)
                    packet.Write(0); // player bank ?

                packet.Write(1); // ar << m_nPlusMaxHitPoint
                packet.Write<byte>(0); // ar << m_nAttackResistLeft			
                packet.Write<byte>(0); // ar << m_nAttackResistRight			
                packet.Write<byte>(0); // ar << m_nDefenseResist
                packet.Write<long>(0); // ar << m_nAngelExp
                packet.Write(0); // ar << m_nAngelLevel

                // Serialize Inventory
                player.Inventory.Serialize(packet);

                // Bank
                for (var i = 0; i < 3; ++i)
                {
                    for (var j = 0; j < 0x2A; ++j)
                        packet.Write(j);
                    packet.Write<byte>(0); // count
                    for (var j = 0; j < 0x2A; ++j)
                        packet.Write(j);
                }

                packet.Write(int.MaxValue); // pet id

                // Bag
                packet.Write<byte>(1);
                for (var i = 0; i < 6; i++)
                    packet.Write(i);
                packet.Write<byte>(0); // Base bag item count
                for (var i = 0; i < 0; i++)
                {
                    packet.Write((byte)i); // Slot
                    packet.Write(i); // Slot
                }
                for (var i = 0; i < 6; i++)
                    packet.Write(i);
                packet.Write(0);
                packet.Write(0);

                // premium bag
                packet.Write<byte>(0);

                packet.Write(0); // muted

                // Honor titles
                for (var i = 0; i < 150; ++i)
                    packet.Write(0);

                packet.Write(0); // id campus
                packet.Write(0); // campus points

                // buffs
                packet.Write(0); // count

                player.Connection.Send(packet);
            }

            // Taskbar
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.TASKBAR);
                player.Taskbar.Serialize(packet);

                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendSpawnObjectTo(IPlayerEntity player, IWorldEntity entityToSpawn)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entityToSpawn.Id, SnapshotType.ADD_OBJ);

                packet.Write((byte)entityToSpawn.Object.Type);
                packet.Write(entityToSpawn.Object.ModelId);
                packet.Write((byte)entityToSpawn.Object.Type);
                packet.Write(entityToSpawn.Object.ModelId);
                packet.Write(entityToSpawn.Object.Size);
                packet.Write(entityToSpawn.Object.Position.X);
                packet.Write(entityToSpawn.Object.Position.Y);
                packet.Write(entityToSpawn.Object.Position.Z);
                packet.Write((short)(entityToSpawn.Object.Angle * 10f));
                packet.Write((int)entityToSpawn.Id);

                if (entityToSpawn.Type == WorldEntityType.Player)
                {
                    var playerEntity = entityToSpawn as IPlayerEntity;

                    packet.Write<short>(0);
                    packet.Write<byte>(1); // is player?
                    packet.Write(playerEntity.Health.Hp); // HP
                    packet.Write((int)playerEntity.Object.MovingFlags); // moving flags
                    packet.Write((int)playerEntity.Object.MotionFlags); // motion flags
                    packet.Write<byte>(0);
                    packet.Write(-1); // baby buffer

                    packet.Write(playerEntity.Object.Name);
                    packet.Write(playerEntity.VisualAppearance.Gender);
                    packet.Write((byte)playerEntity.VisualAppearance.SkinSetId);
                    packet.Write((byte)playerEntity.VisualAppearance.HairId);
                    packet.Write(playerEntity.VisualAppearance.HairColor);
                    packet.Write((byte)playerEntity.VisualAppearance.FaceId);
                    packet.Write(playerEntity.PlayerData.Id);
                    packet.Write((byte)1);
                    packet.Write((short)playerEntity.Attributes[DefineAttributes.STR]); // STR
                    packet.Write((short)playerEntity.Attributes[DefineAttributes.STA]); // STA
                    packet.Write((short)playerEntity.Attributes[DefineAttributes.DEX]); // DEX
                    packet.Write((short)playerEntity.Attributes[DefineAttributes.INT]); // INT
                    packet.Write((short)playerEntity.Object.Level); // Level

                    packet.Write(-1);
                    packet.Write(0);

                    packet.Write<byte>(0); // has guild
                    packet.Write(0); // guild cloak

                    packet.Write<byte>(0); // has party

                    packet.Write((byte)playerEntity.PlayerData.Authority); // Authority
                    packet.Write(0); // mode
                    packet.Write(0); // state mode
                    packet.Write(0x000001F6); // item used ??
                    packet.Write(0); // last pk time.
                    packet.Write(0); // karma
                    packet.Write(0); // pk propensity
                    packet.Write(0); // pk exp
                    packet.Write(0); // fame
                    packet.Write<byte>(0); // duel
                    packet.Write(-1); // titles

                    // Serialize visible effects
                    IEnumerable<Item> equipedItems = playerEntity.Inventory.Items.GetRange(InventorySystem.EquipOffset, InventorySystem.MaxItems - InventorySystem.EquipOffset);

                    foreach (var item in equipedItems)
                        packet.Write(item.Refines);

                    for (var i = 0; i < 28; i++)
                        packet.Write(0);

                    // Serialize Equiped items
                    packet.Write((byte)equipedItems.Count(x => x.Id != -1));

                    foreach (var item in equipedItems)
                    {
                        if (item != null && item.Id > 0)
                        {
                            packet.Write((byte)(item.Slot - InventorySystem.EquipOffset));
                            packet.Write((short)item.Id);
                            packet.Write<byte>(0);
                        }
                    }

                    packet.Write(-1); // pet ?
                    packet.Write(0); // buffs ?
                }
                else if (entityToSpawn.Type == WorldEntityType.Monster)
                {
                    var monsterEntity = entityToSpawn as IMonsterEntity;

                    packet.Write<short>(5);
                    packet.Write<byte>(0);
                    packet.Write(monsterEntity.Health.Hp);
                    packet.Write(1);
                    packet.Write(0);
                    packet.Write((byte)monsterEntity.Data.Belligerence);
                    packet.Write(-1);

                    packet.Write((byte)0);
                    packet.Write(-1);
                    packet.Write((byte)0);
                    packet.Write(0);
                    packet.Write((byte)0);
                    if (entityToSpawn.Object.ModelId == 1021)
                        packet.Write((byte)0);
                    else
                        packet.Write(false ? (byte)1 : (byte)0);
                    packet.Write((byte)0);
                    packet.Write((byte)0);
                    packet.Write(0);
                    packet.Write(monsterEntity.Moves.SpeedFactor); // speed factor
                    packet.Write(0);
                }
                else if (entityToSpawn.Type == WorldEntityType.Npc)
                {
                    packet.Write<short>(1);
                    packet.Write<byte>(0);
                    packet.Write(1); // can be selected
                    packet.Write(1);
                    packet.Write(0);
                    packet.Write<byte>(1);
                    packet.Write(-1);
                    packet.Write<byte>(0); // Npc hair id
                    packet.Write(0); // Npc hair color
                    packet.Write<byte>(0); // Npc Face Id
                    packet.Write(entityToSpawn.Object.Name);
                    packet.Write<byte>(0); // item equiped count
                    packet.Write<byte>(0);
                    packet.Write<byte>(0);
                    packet.Write<byte>(0);
                    packet.Write(0);
                    packet.Write<float>(1); // speed factor
                    packet.Write(0);
                }
                else if (entityToSpawn.Type == WorldEntityType.Drop)
                {
                    var dropItemEntity = entityToSpawn as IItemEntity;

                    dropItemEntity.Drop.Item.Serialize(packet);
                }

                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendDespawnObjectTo(IPlayerEntity player, IWorldEntity entityToDespawn)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entityToDespawn.Id, SnapshotType.DEL_OBJ);

                player.Connection.Send(packet);
            }
        }
    }
}

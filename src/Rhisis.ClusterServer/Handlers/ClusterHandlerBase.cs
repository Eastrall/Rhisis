﻿using Microsoft.EntityFrameworkCore;
using Rhisis.Abstractions.Entities;
using Rhisis.ClusterServer.Abstractions;
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Game.Features;
using Rhisis.Infrastructure.Persistance;
using Rhisis.Protocol;
using Rhisis.Protocol.Packets.Server;
using Rhisis.Protocol.Packets.Server.Cluster;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.ClusterServer.Handlers
{
    public class ClusterHandlerBase
    {
        protected IRhisisDatabase Database { get; }

        protected ClusterHandlerBase(IRhisisDatabase database)
        {
            Database = database;
        }

        /// <summary>
        /// Gets the characters of a given user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Collection of <see cref="ISelectableCharacter"/>.</returns>
        protected IEnumerable<ISelectableCharacter> GetCharacters(int userId)
        {
            IEnumerable<ISelectableCharacter> characters = Database.Characters
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToList()
                .Select(x => new SelectableCharacter
                {
                    Id = x.Id,
                    Name = x.Name,
                    Gender = (GenderType)x.Gender,
                    Level = x.Level,
                    Slot = x.Slot,
                    MapId = x.MapId,
                    PositionX = x.PosX,
                    PositionY = x.PosY,
                    PositionZ = x.PosZ,
                    SkinSetId = x.SkinSetId,
                    HairId = x.HairId,
                    HairColor = (uint)x.HairColor,
                    FaceId = x.FaceId,
                    JobId = x.JobId,
                    Strength = x.Strength,
                    Stamina = x.Stamina,
                    Intelligence = x.Intelligence,
                    Dexterity = x.Dexterity,
                })
                .ToList();

            foreach (ISelectableCharacter character in characters)
            {
                character.EquipedItems = Database.ItemStorage
                    .Include(x => x.Item)
                    .Where(x =>
                        x.CharacterId == character.Id &&
                        x.Slot > Inventory.InventorySize &&
                        x.StorageTypeId == (int)ItemStorageType.Inventory &&
                        !x.IsDeleted)
                    .OrderBy(x => x.Slot)
                    .Select(x => x.Item.GameItemId)
                    .ToList();
            }

            return characters;
        }

        protected void SendPlayerList(IClusterUser user, int authenticationKey)
        {
            IEnumerable<ISelectableCharacter> selectableCharacters = GetCharacters(user.UserId);
            using var playerListPacket = new PlayerListPacket(authenticationKey, selectableCharacters);

            user.Send(playerListPacket);
        }

        protected static void SendError(IClusterUser user, ErrorType errorType)
        {
            using var errorPacket = new ErrorPacket(errorType);
            
            user.Send(errorPacket);
        }
    }
}

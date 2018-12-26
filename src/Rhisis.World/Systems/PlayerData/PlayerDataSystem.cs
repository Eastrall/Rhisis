using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Database;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.PlayerData.EventArgs;
using System.Linq;

namespace Rhisis.World.Systems.PlayerData
{
    [System(SystemType.Notifiable)]
    public class PlayerDataSystem : ISystem
    {
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger<PlayerDataSystem>>();

        public WorldEntityType Type => WorldEntityType.Player;

        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;

            if (!e.CheckArguments())
            {
                Logger.LogError($"Cannot execute player data action {e.GetType()} due to invalid arguments.");
                return;
            }

            switch (e)
            {
                case QueryPlayerDataEventArgs queryPlayerDataEvent:
                    GetPlayerData(playerEntity, queryPlayerDataEvent);
                    break;
                case QueryPlayerData2EventArgs queryPlayerData2Event:
                    GetPlayerData(playerEntity, queryPlayerData2Event);
                    break;
            }
        }

        private void GetPlayerData(IPlayerEntity player, QueryPlayerDataEventArgs e, bool send = true)
        {
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();
            var playerEntity = worldServer.GetPlayerEntity(e.PlayerId);

            // Player is offline
            if (playerEntity is null)
            {
                using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                {
                    var character = database.Characters.Get(x => x.Id == e.PlayerId);
                    WorldPacketFactory.SendPlayerData(player, e.PlayerId, character.Name, (byte)character.ClassId, (byte)character.Level, character.Gender, false, send);
                }
            }
            else // Player is online
                WorldPacketFactory.SendPlayerData(player, e.PlayerId, playerEntity.Object.Name, (byte)playerEntity.PlayerData.JobId, (byte)playerEntity.Object.Level, playerEntity.VisualAppearance.Gender, true, send);
        }

        private void GetPlayerData(IPlayerEntity player, QueryPlayerData2EventArgs e)
        {
            for (int i = 0; i < e.Size; i++)
            {
                var args = new QueryPlayerDataEventArgs(e.PlayerDictionary.Keys.ElementAt(i), e.PlayerDictionary.Values.ElementAt(i));
                GetPlayerData(player, args, i == e.Size - 1);
            }
        }
    }
}

using NLog;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Inventory.EventArgs;

namespace Rhisis.World.Game.Chat
{
    public static class CreateItemChatCommand
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [ChatCommand("/createitem", AuthorityType.Administrator)]
        [ChatCommand("/ci", AuthorityType.Administrator)]
        [ChatCommand("/item", AuthorityType.Administrator)]
        public static void CreateItem(IPlayerEntity player, string[] parameters)
        {
            Logger.Debug("{0} want to create an item", player.Object.Name);

            if (parameters.Length <= 0)
            {
                Logger.Error("Chat: /createitem command must have at least one parameter.");
                return;
            }

            if (!player.Inventory.HasAvailableSlots())
            {
                WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }

            var quantity = 1;

            if (!int.TryParse(parameters[0], out int itemId))
            {
                ItemData itemData = GameResources.Instance.Items[parameters[0]];

                itemId = itemData?.Id ?? -1;
            }

            if (parameters.Length >= 2)
                int.TryParse(parameters[1], out quantity);

            var createItemEvent = new InventoryCreateItemEventArgs(itemId, quantity, player.PlayerData.Id);
            player.NotifySystem<InventorySystem>(createItemEvent);
        }
    }
}
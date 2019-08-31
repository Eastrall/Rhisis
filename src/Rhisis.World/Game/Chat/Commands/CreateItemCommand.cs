using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using System;

namespace Rhisis.World.Game.Chat
{
    [ChatCommand("/createitem", AuthorityType.Administrator)]
    [ChatCommand("/ci", AuthorityType.Administrator)]
    [ChatCommand("/item", AuthorityType.Administrator)]
    public class CreateItemChatCommand : IChatCommand
    {
        private readonly ILogger<CreateItemChatCommand> _logger;
        private readonly IInventorySystem _inventorySystem;
        private readonly IItemFactory _itemFactory;
        private readonly ITextPacketFactory _textPacketFactory;

        /// <summary>
        /// Creates a new <see cref="CreateItemChatCommand"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="itemFactory">Item factory.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public CreateItemChatCommand(ILogger<CreateItemChatCommand> logger, IInventorySystem inventorySystem, IItemFactory itemFactory, ITextPacketFactory textPacketFactory)
        {
            this._logger = logger;
            this._inventorySystem = inventorySystem;
            this._itemFactory = itemFactory;
            this._textPacketFactory = textPacketFactory;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {
            this._logger.LogTrace($"{player.Object.Name} want to create an item");

            if (parameters.Length <= 0)
            {
                throw new ArgumentException($"Create item command must have at least one parameter.", nameof(parameters));
            }

            if (!player.Inventory.HasAvailableSlots())
            {
                this._textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }

            int itemId = Convert.ToInt32(parameters[0]);
            int quantity = parameters.Length >= 2 ? Convert.ToInt32(parameters[1]) : 1;
            byte refine = parameters.Length >= 3 ? Convert.ToByte(parameters[2]) : (byte)0;
            byte element = parameters.Length >= 4 ? Convert.ToByte(parameters[3]) : (byte)0;
            byte elementRefine = parameters.Length >= 5 ? Convert.ToByte(parameters[4]) : (byte)0;
            Item itemToCreate = this._itemFactory.CreateItem(itemId, refine, element, elementRefine, player.PlayerData.Id);

            this._inventorySystem.CreateItem(player, itemToCreate, quantity, player.PlayerData.Id);
        }
    }
}
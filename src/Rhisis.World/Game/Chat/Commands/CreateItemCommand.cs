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
            _logger = logger;
            _inventorySystem = inventorySystem;
            _itemFactory = itemFactory;
            _textPacketFactory = textPacketFactory;
        }

        /// <inheritdoc />
        public void Execute(IPlayerEntity player, object[] parameters)
        {
            _logger.LogTrace($"{player.Object.Name} wants to create an item");

            if (parameters.Length <= 0)
            {
                throw new ArgumentException($"Create item command must have at least one parameter.", nameof(parameters));
            }

            if (!player.Inventory.HasAvailableSlots())
            {
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                return;
            }
            int quantity = parameters.Length >= 2 ? Convert.ToInt32(parameters[1]) : 1;
            byte refine = parameters.Length >= 3 ? Convert.ToByte(parameters[2]) : (byte)0;
            ElementType element = parameters.Length >= 4 ? (ElementType)Enum.Parse(typeof(ElementType), parameters[3].ToString(), true) : default;
            byte elementRefine = parameters.Length >= 5 ? Convert.ToByte(parameters[4]) : (byte)0;

            string itemInput = parameters[0].ToString();
            InventoryItem itemToCreate;
            if (!int.TryParse(itemInput, out int itemId))
            {
                itemToCreate = _itemFactory.CreateInventoryItem(itemInput, refine, element, elementRefine, player.PlayerData.Id);
            }
            else
            {
                itemToCreate = _itemFactory.CreateInventoryItem(itemId, refine, element, elementRefine, player.PlayerData.Id);
            }

            if(itemToCreate != null)
                _inventorySystem.CreateItem(player, itemToCreate, quantity, player.PlayerData.Id);
        }
    }
}

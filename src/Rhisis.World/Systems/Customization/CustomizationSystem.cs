using Microsoft.Extensions.Options;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Configuration.World;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.PlayerData;
using System.Drawing;

namespace Rhisis.World.Systems.Customization
{
    [Injectable]
    public sealed class CustomizationSystem : ICustomizationSystem
    {
        private readonly IInventorySystem _inventorySystem;
        private readonly ICustomizationPacketFactory _customizationPacketFactory;
        private readonly IPlayerDataSystem _playerDataSystem;
        private readonly ITextPacketFactory _textPacketFactory;
        private readonly WorldConfiguration _worldServerConfiguration;

        /// <summary>
        /// Creates a new <see cref="CustomizationSystem"/> instance.
        /// </summary>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="customizationPacketFactory">Customization packet factory.</param>
        /// <param name="worldServerConfiguration">World server configuration.</param>
        /// <param name="playerDataSystem">Player data system.</param>
        /// <param name="textPacketFactory">Text packet factory.</param>
        public CustomizationSystem(IInventorySystem inventorySystem, ICustomizationPacketFactory customizationPacketFactory, IOptions<WorldConfiguration> worldServerConfiguration, IPlayerDataSystem playerDataSystem, ITextPacketFactory textPacketFactory)
        {
            _inventorySystem = inventorySystem;
            _customizationPacketFactory = customizationPacketFactory;
            _playerDataSystem = playerDataSystem;
            _textPacketFactory = textPacketFactory;
            _worldServerConfiguration = worldServerConfiguration.Value;
        }

        /// <inheritdoc />
        public void ChangeFace(IPlayerEntity player, uint objectId, uint faceId, bool useCoupon)
        {
            if (!useCoupon)
            {
                if (player.PlayerData.Gold < _worldServerConfiguration.Customization.ChangeFaceCost)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.VisualAppearance.FaceId = (int)faceId;
                    _playerDataSystem.DecreaseGold(player, (int)_worldServerConfiguration.Customization.ChangeFaceCost);
                    _customizationPacketFactory.SendChangeFace(player, faceId);
                }
            }
            else
            {
                InventoryItem couponItem = player.Inventory.GetItemById(DefineItem.II_SYS_SYS_SCR_FACEOFFFREE);

                if (couponItem == null)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WARNNING_COUPON);
                    return;
                }

                _inventorySystem.DeleteItem(player, couponItem, 1);

                _customizationPacketFactory.SendChangeFace(player, faceId);
            }
        }

        /// <inheritdoc />
        public void ChangeHair(IPlayerEntity player, byte hairId, byte r, byte g, byte b, bool useCoupon)
        {
            if (!useCoupon)
            {
                int costs = 0;
                var color = Color.FromArgb(r, g, b).ToArgb();

                if (player.VisualAppearance.HairId != hairId)
                    costs += (int)_worldServerConfiguration.Customization.ChangeHairCost;
                if (player.VisualAppearance.HairColor != color)
                    costs += (int)_worldServerConfiguration.Customization.ChangeHairColorCost;

                if (player.PlayerData.Gold < costs)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.VisualAppearance.HairId = hairId;
                    player.VisualAppearance.HairColor = color;

                    _playerDataSystem.DecreaseGold(player, costs);
                    _customizationPacketFactory.SendSetHair(player, hairId, r, g, b);
                }
            }
            else
            {
                var couponItem = player.Inventory.GetItemById(DefineItem.II_SYS_SYS_SCR_HAIRCHANGE);
                if (couponItem == null)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WARNNING_COUPON);
                    return;
                }

                _inventorySystem.DeleteItem(player, couponItem, 1);
                _customizationPacketFactory.SendSetHair(player, hairId, r, g, b);
            }
        }
    }
}

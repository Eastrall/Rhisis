using Microsoft.Extensions.Options;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Configuration;
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
        private readonly WorldConfiguration _worldServerConfiguration;

        /// <summary>
        /// Creates a new <see cref="CustomizationSystem"/> instance.
        /// </summary>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="customizationPacketFactory">Customization packet factory.</param>
        /// <param name="worldServerConfiguration">World server configuration.</param>
        /// <param name="playerDataSystem">Player data system.</param>
        public CustomizationSystem(IInventorySystem inventorySystem, ICustomizationPacketFactory customizationPacketFactory, IOptions<WorldConfiguration> worldServerConfiguration, IPlayerDataSystem playerDataSystem)
        {
            this._inventorySystem = inventorySystem;
            this._customizationPacketFactory = customizationPacketFactory;
            this._playerDataSystem = playerDataSystem;
            this._worldServerConfiguration = worldServerConfiguration.Value;
        }

        /// <inheritdoc />
        public void ChangeFace(IPlayerEntity player, uint objectId, uint faceId, bool useCoupon)
        {
            if (!useCoupon)
            {
                if (player.PlayerData.Gold < this._worldServerConfiguration.Customization.ChangeFaceCost)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.VisualAppearance.FaceId = (int)faceId;
                    this._playerDataSystem.DecreaseGold(player, (int)this._worldServerConfiguration.Customization.ChangeFaceCost);
                    this._customizationPacketFactory.SendChangeFace(player, faceId);
                }
            }
            else
            {
                Item couponItem = player.Inventory.GetItemById(DefineItem.II_SYS_SYS_SCR_FACEOFFFREE);

                if (couponItem == null)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WARNNING_COUPON);
                    return;
                }

                this._inventorySystem.DeleteItem(player, couponItem, 1);

                this._customizationPacketFactory.SendChangeFace(player, faceId);
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
                    costs += (int)this._worldServerConfiguration.Customization.ChangeHairCost;
                if (player.VisualAppearance.HairColor != color)
                    costs += (int)this._worldServerConfiguration.Customization.ChangeHairColorCost;

                if (player.PlayerData.Gold < costs)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.VisualAppearance.HairId = hairId;
                    player.VisualAppearance.HairColor = color;

                    this._playerDataSystem.DecreaseGold(player, costs);
                    this._customizationPacketFactory.SendSetHair(player, hairId, r, g, b);
                }
            }
            else
            {
                var couponItem = player.Inventory.GetItemById(DefineItem.II_SYS_SYS_SCR_HAIRCHANGE);
                if (couponItem == null)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WARNNING_COUPON);
                    return;
                }

                this._inventorySystem.DeleteItem(player, couponItem, 1);
                this._customizationPacketFactory.SendSetHair(player, hairId, r, g, b);
            }
        }
    }
}

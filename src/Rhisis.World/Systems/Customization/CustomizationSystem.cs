using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Configuration;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Customization.EventArgs;
using System.Drawing;

namespace Rhisis.World.Systems.Customization
{
    [System(SystemType.Notifiable)]
    public class CustomizationSystem : ISystem
    {
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger>();

        public WorldEntityType Type => WorldEntityType.Player;

        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity playerEntity) || !args.GetCheckArguments())
            {
                Logger.LogError("CustomizationSystem: Invalid arguments.");
                return;
            }

            switch(args)
            {
                case ChangeFaceEventArgs e:
                    OnChangeFace(playerEntity, e);
                    break;
                case SetHairEventArgs e:
                    OnSetHair(playerEntity, e);
                    break;
                default:
                    Logger.LogWarning("Unknown Customization action type: {0} for player {1}", args.GetType(), entity.Object.Name);
                    break;
            }
        }

        private void OnSetHair(IPlayerEntity player, SetHairEventArgs e)
        {
            var worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();

            if (!e.UseCoupon)
            {
                int costs = 0;
                var color = Color.FromArgb(e.R, e.G, e.B).ToArgb();

                if (player.VisualAppearance.HairId != e.HairId)
                    costs += (int)worldConfiguration.Customization.ChangeHairCost;
                if (player.VisualAppearance.HairColor != color)
                    costs += (int)worldConfiguration.Customization.ChangeHairColorCost;

                if (player.PlayerData.Gold < costs)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.PlayerData.Gold -= costs;
                    player.VisualAppearance.HairId = e.HairId;
                    player.VisualAppearance.HairColor = color;

                    WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                    WorldPacketFactory.SendSetHair(player, e.HairId, e.R, e.G, e.B);
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

                player.Inventory.RemoveItems(couponItem.Data.Id);
                WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, couponItem.UniqueId, couponItem.Quantity);
                WorldPacketFactory.SendSetHair(player, e.HairId, e.R, e.G, e.B);
            }
        }

        private void OnChangeFace(IPlayerEntity player, ChangeFaceEventArgs e)
        {
            var worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();

            if(!e.UseCoupon)
            {
                if (player.PlayerData.Gold < worldConfiguration.Customization.ChangeFaceCost)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                }
                else
                {
                    player.PlayerData.Gold -= (int)worldConfiguration.Customization.ChangeFaceCost;
                    player.VisualAppearance.FaceId = (int)e.FaceId;

                    WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                    WorldPacketFactory.SendChangeFace(player, e.FaceId);
                }
            }
            else
            {
                var couponItem = player.Inventory.GetItemById(DefineItem.II_SYS_SYS_SCR_FACEOFFFREE);
                if (couponItem == null)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WARNNING_COUPON);
                    return;
                }

                player.Inventory.RemoveItems(couponItem.Data.Id);
                WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, couponItem.UniqueId, couponItem.Quantity);
                WorldPacketFactory.SendChangeFace(player, e.FaceId);
            }
        }
    }
}

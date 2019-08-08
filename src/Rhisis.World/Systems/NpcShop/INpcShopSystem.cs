using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.NpcShop
{
    public interface INpcShopSystem
    {
        void OpenShop(IPlayerEntity player, uint npcObjectId);

        void CloseShop(IPlayerEntity player);

        void BuyItem(IPlayerEntity player);

        void SellItem(IPlayerEntity player);
    }
}

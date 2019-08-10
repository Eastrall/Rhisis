using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.NpcShop;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    [Handler]
    public sealed class NpcShopHandler
    {
        private readonly INpcShopSystem _npcShopSystem;

        /// <summary>
        /// Creates a new <see cref="NpcShopHandler"/>.
        /// </summary>
        /// <param name="npcShopSystem"></param>
        public NpcShopHandler(INpcShopSystem npcShopSystem)
        {
            this._npcShopSystem = npcShopSystem;
        }

        /// <summary>
        /// Player requests to open a NPC shop.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.OPENSHOPWND)]
        public void OnOpenShopWindow(IWorldClient client, OpenShopWindowPacket packet)
        {
            this._npcShopSystem.OpenShop(client.Player, packet.ObjectId);
        }

        /// <summary>
        /// Player closes a opened NPC shop.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.CLOSESHOPWND)]
        public void OnCloseShopWindow(IWorldClient client)
        {
            this._npcShopSystem.CloseShop(client.Player);
        }

        /// <summary>
        /// Player buys an item from a NPC shop.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.BUYITEM)]
        public void OnBuyItem(IWorldClient client, BuyItemPacket packet)
        {
            var npcShopItem = new NpcShopItemInfo
            {
                ItemId = packet.ItemId,
                Slot = packet.Slot,
                Tab = packet.Tab
            };

            this._npcShopSystem.BuyItem(client.Player, npcShopItem, packet.Quantity);
        }

        /// <summary>
        /// Player sells an item at a NPC shop.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.SELLITEM)]
        public void OnSellItem(IWorldClient client, SellItemPacket packet)
        {
            this._npcShopSystem.SellItem(client.Player, packet.ItemUniqueId, packet.Quantity);
        }
    }
}

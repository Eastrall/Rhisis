using System;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.NpcShop.EventArgs
{
    internal sealed class NpcShopBuyEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the item id to buy.
        /// </summary>
        public int ItemId { get; }

        /// <summary>
        /// Gets the quantity of items to buy.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the tab id where the item is located.
        /// </summary>
        public int Tab { get; }

        /// <summary>
        /// Gets the slot id where the item is located.
        /// </summary>
        public int Slot { get; }

        /// <summary>
        /// Gets the item data.
        /// </summary>
        public ItemData ItemData { get; private set; }

        /// <inheritdoc />
        public NpcShopBuyEventArgs(int itemId, int quantity, int tab, int slot)
        {
            this.ItemId = itemId;
            this.Quantity = quantity;
            this.Tab = tab;
            this.Slot = slot;
        }

        /// <inheritdoc />
        public override bool GetCheckArguments()
        {
            return this.ItemId > 0
                   && this.Quantity > 0 && this.Quantity <= this.ItemData.PackMax
                   && this.Tab >= 0 && this.Tab < ShopData.DefaultTabCount;
        }
    }
}

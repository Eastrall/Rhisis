namespace Rhisis.World.Systems.NpcShop
{
    /// <summary>
    /// Defines an NPC Shop item.
    /// </summary>
    public class NpcShopItemInfo
    {
        /// <summary>
        /// Gets or sets the item id to buy.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the tab id where the item is located.
        /// </summary>
        public int Tab { get; set; }

        /// <summary>
        /// Gets or sets the slot id where the item is located.
        /// </summary>
        public int Slot { get; set; }
    }
}

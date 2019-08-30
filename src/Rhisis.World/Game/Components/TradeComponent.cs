namespace Rhisis.World.Game.Components
{
    public sealed class TradeComponent
    {
        public enum TradeConfirm { Error, Ok }

        public enum TradeState { Item, Ok, Confirm }

        /// <summary>
        /// Gets or sets the target trader id.
        /// </summary>
        public uint TargetId { get; set; }

        /// <summary>
        /// Gets or sets the current items for trade.
        /// </summary>
        public ItemContainerComponent Items { get; }

        /// <summary>
        /// Gets the number of items to trade.
        /// </summary>
        public ushort ItemCount { get; set; }

        /// <summary>
        /// Gets the amount of gold to trade.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Exchange state/progress
        /// </summary>
        public TradeState State { get; set; } = TradeState.Item;

        /// <summary>
        /// Gets a value that indicates if the current entity is already trading.
        /// </summary>
        public bool IsTrading => this.TargetId != 0;

        /// <summary>
        /// Creates a new <see cref="TradeComponent"/> instance.
        /// </summary>
        /// <param name="maxItemCapacity">Maximum amount of items in trade window.</param>
        public TradeComponent(int maxItemCapacity)
        {
            this.Items = new ItemContainerComponent(maxItemCapacity);
        }

        /// <summary>
        /// Resets the trade component.
        /// </summary>
        public void Reset()
        {
            this.TargetId = 0;
            this.ItemCount = 0;
            this.Gold = 0;
            this.State = TradeState.Item;

            foreach (var tradeItem in this.Items.Items)
                tradeItem.ExtraUsed = 0;

            this.Items.Reset();
        }
    }
}

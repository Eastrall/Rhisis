using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Trade
{
    /// <summary>
    /// Provides a mechanism to manage the trade system.
    /// </summary>
    public interface ITradeSystem
    {
        /// <summary>
        /// Initialize the trade component system for the given player.
        /// </summary>
        /// <param name="player">Current player.</param>
        void Initialize(IPlayerEntity player);

        /// <summary>
        /// Requests a trade to the target player using his object id.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="targetObjectId">Target player object id.</param>
        void RequestTrade(IPlayerEntity player, uint targetObjectId);

        /// <summary>
        /// Decline a trade request from a target player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="targetObjectId">Target player object id.</param>
        void DeclineTradeRequest(IPlayerEntity player, uint targetObjectId);

        /// <summary>
        /// Start trading between two players.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="targetObjectId">Target player object id.</param>
        void StartTrade(IPlayerEntity player, uint targetObjectId);

        /// <summary>
        /// Put an item to the trade item container.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="itemUniqueId">Item unique id.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="itemType">Item type.</param>
        /// <param name="destinationSlot">Destination slot.</param>
        void PutItem(IPlayerEntity player, int itemUniqueId, int quantity, int itemType, int destinationSlot);

        /// <summary>
        /// Put gold to the trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="goldAmount">Gold amount to place in the trade.</param>
        void PutGold(IPlayerEntity player, int goldAmount);

        /// <summary>
        /// Cancels the trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="mode">Cancel mode.</param>
        void CancelTrade(IPlayerEntity player, int mode);

        /// <summary>
        /// Confirms the trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        void ConfirmTrade(IPlayerEntity player);

        /// <summary>
        /// Last confirmation of the trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        void LastConfirmTrade(IPlayerEntity player);
    }
}

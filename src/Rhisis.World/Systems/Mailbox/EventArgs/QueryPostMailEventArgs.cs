using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Inventory;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class QueryPostMailEventArgs : SystemEventArgs
    {

        /// <summary>
        /// Gets the slot of the item.
        /// </summary>
        public byte ItemSlot { get; }

        /// <summary>
        /// Gets the quantity of the item.
        /// </summary>
        public short ItemQuantity { get; }

        /// <summary>
        /// Gets the receiver's name.
        /// </summary>
        public string Receiver { get; }

        /// <summary>
        /// Gets the amount of gold.
        /// </summary>
        public int Gold { get; }

        /// <summary>
        /// Gets the title of the mail.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the text of the mail.
        /// </summary>
        public string Text { get; }

        public QueryPostMailEventArgs(byte itemSlot, short itemQuantity, string receiver, int gold, string title, string text)
        {
            this.ItemSlot = ItemSlot;
            this.ItemQuantity = itemQuantity;
            this.Receiver = receiver;
            this.Gold = gold;
            this.Title = title;
            this.Text = text;
        }

        public override bool CheckArguments()
        {
            return this.ItemQuantity > 0 && this.ItemSlot < InventorySystem.InventorySize && this.Gold >= 0;
        }
    }
}

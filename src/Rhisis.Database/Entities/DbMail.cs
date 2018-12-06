using System.ComponentModel.DataAnnotations.Schema;

namespace Rhisis.Database.Entities
{
    [Table("mails")]
    public sealed class DbMail : DbEntity
    {
        public DbCharacter Sender { get; set; }
        public DbCharacter Receiver { get; set; }
        public int Gold { get; set; }
        public DbItem Item { get; set; }
        public short ItemQuantity { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool HasBeenRead { get; set; }

        public DbMail()
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, string title, string text)
            : this(sender, receiver, 0, null, 0, title, text, false)
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, DbItem item, short itemQuantity, string title, string text)
            : this(sender, receiver, 0, item, itemQuantity, title, text, false)
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, int gold, string title, string text)
            : this(sender, receiver, gold, null, 0, title, text, false)
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, int gold, DbItem item, short itemQuantity, string title, string text)
            : this(sender, receiver, gold, item, itemQuantity, title, text, false)
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, int gold, DbItem item, short itemQuantity, string title, string text, bool hasBeenRead)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Gold = gold;
            this.Item = item;
            this.ItemQuantity = itemQuantity;
            this.Title = title;
            this.Text = text;
            this.HasBeenRead = hasBeenRead;
        }
    }
}

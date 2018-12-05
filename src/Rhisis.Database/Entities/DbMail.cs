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
        public byte ItemQuantity { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public DbMail()
        {
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, string title, string text)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Title = title;
            this.Text = text;
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, int gold, string title, string text)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Gold = gold;
            this.Title = title;
            this.Text = text;
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, DbItem item, byte itemQuantity, string title, string text)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Item = item;
            this.ItemQuantity = itemQuantity;
            this.Title = title;
            this.Text = text;
        }

        public DbMail(DbCharacter sender, DbCharacter receiver, int gold, DbItem item, byte itemQuantity, string title,
                      string text)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Gold = gold;
            this.Item = item;
            this.ItemQuantity = itemQuantity;
            this.Title = title;
            this.Text = text;
        }
    }
}

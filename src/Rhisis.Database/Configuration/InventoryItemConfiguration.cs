using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class InventoryItemConfiguration : IEntityTypeConfiguration<DbInventoryItem>
    {
        public void Configure(EntityTypeBuilder<DbInventoryItem> builder)
        {
            builder.ToTable("InventoryItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CharacterId).IsRequired();
            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.Slot).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            builder.HasOne(x => x.Character)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.CharacterId, x.Slot }).IsUnique();
        }
    }
}

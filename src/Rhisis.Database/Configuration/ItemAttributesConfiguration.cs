using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class ItemAttributesConfiguration : IEntityTypeConfiguration<DbItemAttributes>
    {
        public void Configure(EntityTypeBuilder<DbItemAttributes> builder)
        {
            builder.ToTable("ItemAttributes");
            builder.HasKey(x => new { x.ItemId, x.AttributeId });
            builder.HasIndex(x => new { x.ItemId, x.AttributeId }).IsUnique();
            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.AttributeId).IsRequired();
            builder.Property(x => x.Value).IsRequired();
            builder.HasOne(x => x.Item)
                .WithMany(x => x.ItemAttributes)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Attribute)
                .WithMany()
                .HasForeignKey(x => x.AttributeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

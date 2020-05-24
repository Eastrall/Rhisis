using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class ItemConfiguration : IEntityTypeConfiguration<DbItem>
    {
        public void Configure(EntityTypeBuilder<DbItem> builder)
        {
            builder.ToTable("Items");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.Refine).HasColumnType("TINYINT").IsRequired(false);
            builder.Property(x => x.Element).HasColumnType("TINYINT").IsRequired(false);
            builder.Property(x => x.ElementRefine).HasColumnType("TINYINT").IsRequired(false);
            builder.HasMany(x => x.ItemAttributes).WithOne(x => x.Item).HasForeignKey(x => x.ItemId);
        }
    }
}

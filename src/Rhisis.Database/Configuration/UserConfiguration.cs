using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.ToTable("Users");
            builder.HasIndex(c => new { c.Username, c.Email }).IsUnique();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Username).IsRequired().HasMaxLength(32).HasColumnType("NVARCHAR(32)");
            builder.Property(x => x.Password).IsRequired().HasMaxLength(32).HasColumnType("VARCHAR(32)");
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255).HasColumnType("VARCHAR(255)"); // TODO: add fluent encryption
            builder.Property(x => x.EmailConfirmed).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.LastConnectionTime).IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.Authority).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.IsDeleted).IsRequired().HasColumnType("BIT");
            builder.Ignore(x => x.PlayTime);
            builder.HasMany(x => x.Characters)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

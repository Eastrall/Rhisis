using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class CharacterConfiguration : IEntityTypeConfiguration<DbCharacter>
    {
        public void Configure(EntityTypeBuilder<DbCharacter> builder)
        {
            builder.ToTable("Characters");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(32).HasColumnType("NVARCHAR(32)");
            builder.Property(x => x.Gender).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.Level).IsRequired().HasDefaultValue(1);
            builder.Property(x => x.Experience).HasColumnType("BIGINT").HasDefaultValue(0);
            builder.Property(x => x.JobId).IsRequired().HasColumnType("TINYINT").HasDefaultValue(1);
            builder.Property(x => x.Experience).HasColumnType("BIGINT").HasDefaultValue(0);
            builder.Property(x => x.Slot).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.Strength).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.Stamina).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.Dexterity).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.Intelligence).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.Hp).IsRequired();
            builder.Property(x => x.Mp).IsRequired();
            builder.Property(x => x.Fp).IsRequired();
            builder.Property(x => x.SkinSetId).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.HairColor).IsRequired();
            builder.Property(x => x.HairId).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.FaceId).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.MapId).IsRequired();
            builder.Property(x => x.MapLayerId).IsRequired();
            builder.Property(x => x.PosX).IsRequired();
            builder.Property(x => x.PosY).IsRequired();
            builder.Property(x => x.PosZ).IsRequired();
            builder.Property(x => x.Angle).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.BankCode).IsRequired().HasColumnType("SMALLINT(4)");
            builder.Property(x => x.StatPoints).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.SkillPoints).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.LastConnectionTime).IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.PlayTime).IsRequired().HasColumnType("BIGINT").HasDefaultValue(0);
            builder.Property(x => x.IsDeleted).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.ClusterId).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.UserId).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Characters)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TaskbarShortcuts)
                .WithOne(x => x.Character)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ReceivedMails).WithOne(x => x.Receiver);
            builder.HasMany(x => x.SentMails).WithOne(x => x.Sender);
        }
    }
}

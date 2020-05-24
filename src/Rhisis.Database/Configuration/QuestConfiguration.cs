using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class QuestConfiguration : IEntityTypeConfiguration<DbQuest>
    {
        public void Configure(EntityTypeBuilder<DbQuest> builder)
        {
            builder.ToTable("Quests");
            builder.HasIndex(x => new { x.QuestId, x.CharacterId }).IsUnique();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.QuestId).IsRequired();
            builder.Property(x => x.Finished).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.IsChecked).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.IsDeleted).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.StartTime).IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.IsPatrolDone).IsRequired().HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.MonsterKilled1).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.MonsterKilled2).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.CharacterId).IsRequired();
            builder.HasOne(x => x.Character)
                .WithMany()
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Configuration
{
    internal class SkillConfiguration : IEntityTypeConfiguration<DbSkill>
    {
        public void Configure(EntityTypeBuilder<DbSkill> builder)
        {
            builder.ToTable("Skills");
            builder.HasIndex(x => new { x.SkillId, x.CharacterId }).IsUnique();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.SkillId).IsRequired();
            builder.Property(x => x.Level).IsRequired().HasColumnType("TINYINT");
            builder.Property(x => x.CharacterId).IsRequired();
            builder.HasOne(x => x.Character)
                .WithMany()
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

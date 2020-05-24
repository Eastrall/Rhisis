using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rhisis.Core.Data;
using Rhisis.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Database.Configuration
{
    internal class AttributeConfiguration : IEntityTypeConfiguration<DbAttribute>
    {
        public void Configure(EntityTypeBuilder<DbAttribute> builder)
        {
            builder.ToTable("Attributes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(20).HasColumnType("VARCHAR(20)");

            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<DbAttribute> builder)
        {
            IEnumerable<DbAttribute> initialValues = Enum.GetValues(typeof(DefineAttributes))
                   .Cast<DefineAttributes>()
                   .Select(x => new DbAttribute
                   {
                       Id = (int)x,
                       Name = x.ToString()
                   });

            builder.HasData(initialValues);
        }
    }
}

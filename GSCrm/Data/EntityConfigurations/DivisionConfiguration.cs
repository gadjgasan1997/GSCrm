using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class DivisionConfiguration : IEntityTypeConfiguration<Division>
    {
        public void Configure(EntityTypeBuilder<Division> modelBuilder)
        {
            modelBuilder
                .HasMany(p => p.Positions)
                .WithOne(div => div.Division)
                .HasForeignKey(divId => divId.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> modelBuilder)
        {
            modelBuilder
                .HasMany(empPos => empPos.EmployeePositions)
                .WithOne(pos => pos.Position)
                .HasForeignKey(posId => posId.PositionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

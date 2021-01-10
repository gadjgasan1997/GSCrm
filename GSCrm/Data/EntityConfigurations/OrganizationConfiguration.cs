using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> modelBuilder)
        {
            modelBuilder
                .HasMany(div => div.Divisions)
                .WithOne(org => org.Organization)
                .HasForeignKey(orgId => orgId.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .HasMany(pos => pos.Positions)
                .WithOne(org => org.Organization)
                .HasForeignKey(orgId => orgId.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(emp => emp.Employees)
                .WithOne(org => org.Organization)
                .HasForeignKey(orgId => orgId.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(userOrgs => userOrgs.UserOrganizations)
                .WithOne(org => org.Organization)
                .HasForeignKey(orgId => orgId.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GSCrm.Data.EntityConfigurations
{
    public class UserOrganizationConfiguration : IEntityTypeConfiguration<UserOrganization>
    {
        public void Configure(EntityTypeBuilder<UserOrganization> modelBuilder)
        {
            modelBuilder
                .HasOne(u => u.User)
                .WithMany(usOrgs => usOrgs.UserOrganizations)
                .HasForeignKey(id => id.UserId);
        }
    }
}

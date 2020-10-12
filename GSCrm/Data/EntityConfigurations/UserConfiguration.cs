using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GSCrm.Data.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder
                .HasMany(userOrgs => userOrgs.UserOrganizations)
                .WithOne(u => u.User)
                .HasForeignKey(uId => uId.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Преобразование перечисления
            modelBuilder.Property(lang
                => lang.DefaultLanguage).HasConversion(
                lang => lang.ToString(),
                lang => (LangType)Enum.Parse(typeof(LangType), lang));
        }
    }
}

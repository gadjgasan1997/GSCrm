using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class AccountManagerConfiguration : IEntityTypeConfiguration<AccountManager>
    {
        public void Configure(EntityTypeBuilder<AccountManager> modelBuilder)
        {
            modelBuilder.HasAlternateKey(u => new { u.AccountId, u.ManagerId }).HasName("ManagersUniqueKey");
        }
    }
}

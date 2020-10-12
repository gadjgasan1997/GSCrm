using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> modelBuilder)
        {
            modelBuilder
                .HasMany(ac => ac.AccountContacts)
                .WithOne(acc => acc.Account)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(add => add.AccountAddresses)
                .WithOne(acc => acc.Account)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(i => i.AccountInvoices)
                .WithOne(acc => acc.Account)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(q => q.AccountQuotes)
                .WithOne(acc => acc.Account)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .HasMany(accMan => accMan.AccountManagers)
                .WithOne(acc => acc.Account)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSCrm.Data.EntityConfigurations
{
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> modelBuilder)
        {
            modelBuilder
                .HasMany(ac => ac.AccountQuotes)
                .WithOne(acc => acc.Quote)
                .HasForeignKey(acId => acId.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

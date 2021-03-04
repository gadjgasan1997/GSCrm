using System;
using GSCrm.Models;
using GSCrm.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class AccountAddressConfiguration : IEntityTypeConfiguration<AccountAddress>
    {
        public void Configure(EntityTypeBuilder<AccountAddress> modelBuilder)
        {
            // Преобразование перечисления
            modelBuilder.Property(addr
                => addr.AddressType).HasConversion(
                addr => addr.ToString(),
                addr => (AddressType)Enum.Parse(typeof(AddressType), addr));
        }
    }
}

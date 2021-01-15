using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> modelBuilder)
        {
            modelBuilder
                .HasMany(prod => prod.Products)
                .WithOne(prodCat => prodCat.ProductCategory)
                .HasForeignKey(prodCat => prodCat.ProductCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

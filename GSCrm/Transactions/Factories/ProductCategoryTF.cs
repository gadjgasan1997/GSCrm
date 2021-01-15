using GSCrm.Data;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class ProductCategoryTF : TransactionFactory<ProductCategoryViewModel>
    {
        public ProductCategoryTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}

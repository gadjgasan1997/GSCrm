using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Transactions.Factories
{
    public class OrganizationTF : TransactionFactory<OrganizationViewModel>
    {
        public OrganizationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}

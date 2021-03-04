using System;
using GSCrm.Data;
using GSCrm.Models.ViewModels;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeResponsibilityTF : TransactionFactory<EmployeeResponsibilityViewModel>
    {
        public EmployeeResponsibilityTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}

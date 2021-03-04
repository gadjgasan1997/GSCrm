using System;
using GSCrm.Data;
using GSCrm.Models.ViewModels;

namespace GSCrm.Transactions.Factories
{
    public class EmployeePositionTF : TransactionFactory<EmployeePositionViewModel>
    {
        public EmployeePositionTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}

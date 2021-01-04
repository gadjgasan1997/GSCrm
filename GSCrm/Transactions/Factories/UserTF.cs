using GSCrm.Data;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class UserTF : TransactionFactory<UserViewModel>
    {
        public UserTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}

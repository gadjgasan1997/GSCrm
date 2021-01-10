using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Models;
using GSCrm.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GSCrm.Transactions
{
    public static class TransactionsUtils
    {
        public static void RememberAccountCommonParams(this ITransaction transaction, ICachService cachService, ApplicationDbContext context, User currentUser)
        {
            Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
            transaction.AddParameter("CurrentAccount", currentAccount);
            Organization ownerOrg = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == currentAccount.OrganizationId);
            transaction.AddParameter("OwnerOrg", ownerOrg);
        }
    }
}

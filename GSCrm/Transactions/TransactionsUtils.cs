using System.Linq;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Transactions
{
    public static class TransactionsUtils
    {
        public static void RememberAccountCommonParams(this ITransaction transaction, ICachService cachService, ApplicationDbContext context, User currentUser)
        {
            Account currentAccount = cachService.GetCachedCurrentEntity<Account>(currentUser);
            Organization ownerOrg = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == currentAccount.OrganizationId);
            transaction.AddParameter("OwnerOrg", ownerOrg);
        }
    }
}

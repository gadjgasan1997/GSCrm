using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class AccountContactHelpers
    {
        public static Account GetAccount(this AccountContact accountContact, ApplicationDbContext context)
            => context.Accounts.AsNoTracking().FirstOrDefault(acc => acc.Id == accountContact.AccountId);
    }
}

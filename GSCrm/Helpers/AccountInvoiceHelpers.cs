using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Helpers
{
    public static class AccountInvoiceHelpers
    {
        public static Account GetAccount(this AccountInvoice accountInvoice, ApplicationDbContext context)
            => context.Accounts.AsNoTracking().FirstOrDefault(acc => acc.Id == accountInvoice.AccountId);
    }
}

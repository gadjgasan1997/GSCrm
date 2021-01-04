using GSCrm.Models;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Transactions
{
    public class TransactionChange
    {
        public object Entity { get; set; }
        public EntityState EntityState { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class AccountManager : BaseDataModel
    {
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Manager")]
        public Guid ManagerId { get; set; }
        public Employee Manager { get; set; }
    }
}

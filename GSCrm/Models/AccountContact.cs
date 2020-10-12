using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class AccountContact : Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class AccountInvoice : BaseDataModel
    {
        public string BankName { get; set; }
        public string City { get; set; }
        public string CheckingAccount { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIC { get; set; }
        public string SWIFT { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}

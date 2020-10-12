using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class AccountQuote : BaseDataModel
    {
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Quote")]
        public Guid QuoteId { get; set; }
        public Quote Quote { get; set; }
    }
}

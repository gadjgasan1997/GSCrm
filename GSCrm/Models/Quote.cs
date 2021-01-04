using GSCrm.Models.Enums;
using System;
using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Quote : BaseDataModel
    {
        public Guid OrganizationId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ManagerId { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public QuoteStatus QuoteStatus { get; set; }

        public List<AccountQuote> AccountQuotes { get; set; }
        public Quote()
        {
            AccountQuotes = new List<AccountQuote>();
        }
    }
}

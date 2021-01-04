using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Mapping
{
    public class AccountQuoteMap : BaseMap<AccountQuote, AccountQuoteViewModel>
    {
        public AccountQuoteMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context)
        { }
    }
}

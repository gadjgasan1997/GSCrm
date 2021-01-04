using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using System;
using GSCrm.Data;

namespace GSCrm.Repository
{
    public class AccountQuoteRepository : BaseRepository<AccountQuote, AccountQuoteViewModel>
    {
        public AccountQuoteRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base (serviceProvider, context)
        { }
    }
}

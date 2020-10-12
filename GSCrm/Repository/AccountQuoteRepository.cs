using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSCrm.Repository
{
    public class AccountQuoteRepository : GenericRepository<AccountQuote, AccountQuoteViewModel, AccountQuoteValidator, AccountQuoteTransformer>
    {
        public AccountQuoteRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base (context, viewsInfo, resManager, new AccountQuoteValidator(context, resManager), new AccountQuoteTransformer(context, resManager))
        { }
    }
}

using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSCrm.DataTransformers
{
    public class AccountQuoteTransformer : BaseTransformer<AccountQuote, AccountQuoteViewModel>
    {
        public AccountQuoteTransformer(ApplicationDbContext context, ResManager resManager) : base (context, resManager)
        { }
    }
}

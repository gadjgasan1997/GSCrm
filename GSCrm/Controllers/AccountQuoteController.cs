using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_QUOTE)]
    public class AccountQuoteController
        : MainController<AccountQuote, AccountQuoteViewModel, AccountQuoteValidator, AccountQuoteTransformer, AccountQuoteRepository>
    {
        public AccountQuoteController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountQuoteTransformer(context, resManager), new AccountQuoteRepository(context, viewsInfo, resManager))
        { }
    }
}

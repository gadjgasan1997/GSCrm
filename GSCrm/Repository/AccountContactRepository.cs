using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class AccountContactRepository : GenericRepository<AccountContact, AccountContactViewModel, AccountContactValidatior, AccountContactTransformer>
    {
        public AccountContactRepository(ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        { }
        public AccountContactRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountContactValidatior(context, resManager), new AccountContactTransformer(context, resManager))
        { }
    }
}

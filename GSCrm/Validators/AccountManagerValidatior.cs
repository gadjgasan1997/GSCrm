using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;

namespace GSCrm.Validators
{
    public class AccountManagerValidatior : BaseValidator<AccountManagerViewModel>
    {
        public AccountManagerValidatior(ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        { }
    }
}

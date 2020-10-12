using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSCrm.Validators
{
    public class AccountQuoteValidator : BaseValidator<AccountQuoteViewModel>
    {
        public AccountQuoteValidator(ApplicationDbContext context, ResManager resManager) : base (context, resManager)
        { }
    }
}

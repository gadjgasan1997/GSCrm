using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;

namespace GSCrm.Repository
{
    public class AccountInvoiceRepository : GenericRepository<AccountInvoice, AccountInvoiceViewModel, AccountInvoiceValidatior, AccountInvoiceTransformer>
    {
        public AccountInvoiceRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountInvoiceValidatior(context, resManager), new AccountInvoiceTransformer(context, resManager))
        { }
    }
}

using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class AccountInvoiceTransformer : BaseTransformer<AccountInvoice, AccountInvoiceViewModel>
    {
        public AccountInvoiceTransformer(ApplicationDbContext context, ResManager resManager)
            : base (context, resManager) { }

        public override AccountInvoiceViewModel DataToViewModel(AccountInvoice dataModel)
        {
            return new AccountInvoiceViewModel()
            {
                Id = dataModel.Id,
                AccountId = dataModel.AccountId.ToString(),
                BankName = dataModel.BankName,
                City = dataModel.City,
                BIC = dataModel.BIC,
                SWIFT = dataModel.SWIFT.ToUpper(),
                CheckingAccount = dataModel.CheckingAccount,
                CorrespondentAccount = dataModel.CorrespondentAccount
            };
        }

        public override AccountInvoice OnModelCreate(AccountInvoiceViewModel invoiceViewModel)
        {
            return new AccountInvoice()
            {
                AccountId = Guid.Parse(invoiceViewModel.AccountId),
                BankName = invoiceViewModel.BankName,
                City = invoiceViewModel.City,
                BIC = invoiceViewModel.BIC,
                SWIFT = invoiceViewModel.SWIFT.ToUpper(),
                CheckingAccount = invoiceViewModel.CheckingAccount,
                CorrespondentAccount = invoiceViewModel.CorrespondentAccount
            };
        }

        public override AccountInvoice OnModelUpdate(AccountInvoiceViewModel invoiceViewModel)
        {
            AccountInvoice accountInvoice = context.AccountInvoices.FirstOrDefault(i => i.Id == invoiceViewModel.Id);
            accountInvoice.BankName = invoiceViewModel.BankName;
            accountInvoice.City = invoiceViewModel.City;
            accountInvoice.BIC = invoiceViewModel.BIC;
            accountInvoice.SWIFT = invoiceViewModel.SWIFT;
            accountInvoice.CheckingAccount = invoiceViewModel.CheckingAccount;
            accountInvoice.CorrespondentAccount = invoiceViewModel.CorrespondentAccount;
            return accountInvoice;
        }
    }
}

using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;
using GSCrm.Data;

namespace GSCrm.Mapping
{
    public class AccountInvoiceMap : BaseMap<AccountInvoice, AccountInvoiceViewModel>
    {
        public AccountInvoiceMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context) { }

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
            base.OnModelCreate(invoiceViewModel);
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
            AccountInvoice accountInvoice = base.OnModelUpdate(invoiceViewModel);
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

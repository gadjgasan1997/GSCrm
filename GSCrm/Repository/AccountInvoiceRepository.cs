using System;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels;
using static GSCrm.RegexConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class AccountInvoiceRepository : BaseRepository<AccountInvoice, AccountInvoiceViewModel>
    {
        #region Declarations
        private readonly int SWIFT_LENGTH_ONE = 8;
        private readonly int SWIFT_LENGTH_TWO = 11;
        private readonly int BIC_LENGTH = 9;
        #endregion

        #region Constructs
        public AccountInvoiceRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(AccountInvoiceViewModel invoiceViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccInvoiceCreate");

        protected override bool TryCreatePrepare(AccountInvoiceViewModel invoiceViewModel)
        {
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(invoiceViewModel),
                () => {
                    if (account.GetInvoices(context).FirstOrDefault(inv => inv.CheckingAccount == invoiceViewModel.CheckingAccount) != null)
                        errors.Add("CheckingAccountAlreadyExists", resManager.GetString("CheckingAccountAlreadyExists"));
                },
                () => {
                    if (account.GetInvoices(context).FirstOrDefault(inv => inv.CorrespondentAccount == invoiceViewModel.CorrespondentAccount) != null)
                        errors.Add("CorrespondentAccountAlreadyExists", resManager.GetString("CorrespondentAccountAlreadyExists"));
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(AccountInvoiceViewModel invoiceViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccInvoiceUpdate");

        protected override bool TryUpdatePrepare(AccountInvoiceViewModel invoiceViewModel)
        {
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(invoiceViewModel),
                () => {
                    AccountInvoice accountInvoice = account.GetInvoices(context).FirstOrDefault(inv => inv.CheckingAccount == invoiceViewModel.CheckingAccount);
                    if (accountInvoice != null && accountInvoice.Id != invoiceViewModel.Id)
                        errors.Add("CheckingAccountAlreadyExists", resManager.GetString("CheckingAccountAlreadyExists"));
                },
                () => {
                    AccountInvoice accountInvoice = account.GetInvoices(context).FirstOrDefault(inv => inv.CorrespondentAccount == invoiceViewModel.CorrespondentAccount);
                    if (accountInvoice != null && accountInvoice.Id != invoiceViewModel.Id)
                        errors.Add("CorrespondentAccountAlreadyExists", resManager.GetString("CorrespondentAccountAlreadyExists"));
                }
            });
            return !errors.Any();
        }

        protected override void UpdateCacheOnDelete(AccountInvoice accountInvoice)
        {
            if (cachService.TryGetCachedEntity(currentUser, accountInvoice.AccountId, out Account account) &&
                cachService.TryGetCachedEntity(currentUser, accountInvoice.AccountId, out AccountViewModel accountViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, account);
                cachService.CacheCurrentEntity(currentUser, accountViewModel);
            }
        }

        protected override bool RespsIsCorrectOnDelete(AccountInvoice invoiceDataModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccInvoiceDelete");
        #endregion

        #region Validations
        /// <summary>
        /// Общие проверки для методов "UpdateCheck" or "CreationCheck"
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CommonChecks(AccountInvoiceViewModel invoiceViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.BankName))
                        errors.Add("BankNameLength", resManager.GetString("BankNameLength"));
                },
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.City))
                        errors.Add("BankCityLength", resManager.GetString("BankCityLength"));
                },
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.SWIFT) || (invoiceViewModel.SWIFT.Length != SWIFT_LENGTH_ONE && invoiceViewModel.SWIFT.Length != SWIFT_LENGTH_TWO))
                        errors.Add("SWIFTLength", resManager.GetString("SWIFTLength"));
                },
                () => {
                    if (LATIN_LETTERS_AND_DIGITS.IsMatch(invoiceViewModel.SWIFT))
                        errors.Add("SWIFWrong", resManager.GetString("SWIFWrong"));
                },
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.BIC) || invoiceViewModel.BIC.Length != BIC_LENGTH)
                        errors.Add("BICLength", resManager.GetString("BICLength"));
                },
                () => {
                    if (ONLY_DIGITS.IsMatch(invoiceViewModel.BIC))
                        errors.Add("BICWrong", resManager.GetString("BICWrong"));
                },
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.CheckingAccount))
                        errors.Add("CheckingAccountLength", resManager.GetString("CheckingAccountLength"));
                },
                () => {
                    if (string.IsNullOrEmpty(invoiceViewModel.CorrespondentAccount))
                        errors.Add("CorrespondentAccountLength", resManager.GetString("CorrespondentAccountLength"));
                }
            });
        }
        #endregion
    }
}

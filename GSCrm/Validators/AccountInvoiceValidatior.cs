using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.RegexConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class AccountInvoiceValidatior : BaseValidator<AccountInvoiceViewModel>
    {
        private readonly int SWIFT_LENGTH_ONE = 8;
        private readonly int SWIFT_LENGTH_TWO = 11;
        private readonly int BIC_LENGTH = 9;
        private Account account;
        public AccountInvoiceValidatior(ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        { }

        public override Dictionary<string, string> CreationCheck(AccountInvoiceViewModel invoiceViewModel)
        {
            if (!TrySetAccount(invoiceViewModel)) return errors;
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CommonChecks(invoiceViewModel),
                () => CheckCheckingAccountOnCreate(invoiceViewModel),
                () => CheckCorrespondentAccountOnCreate(invoiceViewModel)
            });
            return errors;
        }

        public override Dictionary<string, string> UpdateCheck(AccountInvoiceViewModel invoiceViewModel)
        {
            if (!TrySetAccount(invoiceViewModel)) return errors;
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CommonChecks(invoiceViewModel),
                () => CheckCheckingAccountOnUpdate(invoiceViewModel),
                () => CheckCorrespondentAccountOnUpdate(invoiceViewModel)
            });
            return errors;
        }

        /// <summary>
        /// Общие проверки для методов "UpdateCheck" or "CreationCheck"
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CommonChecks(AccountInvoiceViewModel invoiceViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckBankName(invoiceViewModel),
                () => CheckCity(invoiceViewModel),
                () => CheckSWIFT(invoiceViewModel),
                () => CheckBIC(invoiceViewModel),
                () => CheckCheckingAccountlength(invoiceViewModel),
                () => CheckCorrespondentAccountlength(invoiceViewModel),
            });
        }

        /// <summary>
        /// Метод проверяет название банка
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckBankName(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.BankName))
                errors.Add("BankNameLength", resManager.GetString("BankNameLength"));
        }

        /// <summary>
        /// Метод проверяет город
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCity(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.City))
                errors.Add("BankCityLength", resManager.GetString("BankCityLength"));
        }

        /// <summary>
        /// Метод проверяет название SWIFT
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckSWIFT(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.SWIFT) || (invoiceViewModel.SWIFT.Length != SWIFT_LENGTH_ONE && invoiceViewModel.SWIFT.Length != SWIFT_LENGTH_TWO))
            {
                errors.Add("SWIFTLength", resManager.GetString("SWIFTLength"));
                return;
            }
            if (LATIN_LETTERS_AND_DIGITS.IsMatch(invoiceViewModel.SWIFT))
                errors.Add("SWIFWrong", resManager.GetString("SWIFWrong"));
        }

        /// <summary>
        /// Метод проверяет название БИК
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckBIC(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.BIC) || invoiceViewModel.BIC.Length != BIC_LENGTH)
            {
                errors.Add("BICLength", resManager.GetString("BICLength"));
                return;
            }
            if (ONLY_DIGITS.IsMatch(invoiceViewModel.BIC))
                errors.Add("BICWrong", resManager.GetString("BICWrong"));
        }

        /// <summary>
        /// Метод пытается установить найти и запомнить клиента по id из модели
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private bool TrySetAccount(AccountInvoiceViewModel invoiceViewModel)
        {
            if (!Guid.TryParse(invoiceViewModel.AccountId, out Guid accountId))
            {
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
                return false;
            }

            account = context.Accounts.Include(inv => inv.AccountInvoices).FirstOrDefault(i => i.Id == accountId);
            if (account == null)
            {
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка расчетного счета на длину
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCheckingAccountlength(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.CheckingAccount))
                errors.Add("CheckingAccountLength", resManager.GetString("CheckingAccountLength"));
        }

        /// <summary>
        /// Проверка корреспонденского счета на длину
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCorrespondentAccountlength(AccountInvoiceViewModel invoiceViewModel)
        {
            if (string.IsNullOrEmpty(invoiceViewModel.CorrespondentAccount))
                errors.Add("CorrespondentAccountLength", resManager.GetString("CorrespondentAccountLength"));
        }

        /// <summary>
        /// Метод проверяет название расчетный счет
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCheckingAccountOnCreate(AccountInvoiceViewModel invoiceViewModel)
        {
            if (account.AccountInvoices.FirstOrDefault(inv => inv.CheckingAccount == invoiceViewModel.CheckingAccount) != null)
                errors.Add("CheckingAccountAlreadyExists", resManager.GetString("CheckingAccountAlreadyExists"));
        }

        /// <summary>
        /// Метод проверяет название корреспонденский счет
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCorrespondentAccountOnCreate(AccountInvoiceViewModel invoiceViewModel)
        {
            if (account.AccountInvoices.FirstOrDefault(inv => inv.CorrespondentAccount == invoiceViewModel.CorrespondentAccount) != null)
                errors.Add("CorrespondentAccountAlreadyExists", resManager.GetString("CorrespondentAccountAlreadyExists"));
        }

        /// <summary>
        /// Метод проверяет название расчетный счет
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCheckingAccountOnUpdate(AccountInvoiceViewModel invoiceViewModel)
        {
            AccountInvoice accountInvoice = account.AccountInvoices.FirstOrDefault(inv => inv.CheckingAccount == invoiceViewModel.CheckingAccount);
            if (accountInvoice != null && accountInvoice.Id != invoiceViewModel.Id)
                errors.Add("CheckingAccountAlreadyExists", resManager.GetString("CheckingAccountAlreadyExists"));
        }

        /// <summary>
        /// Метод проверяет название корреспонденский счет
        /// </summary>
        /// <param name="invoiceViewModel"></param>
        private void CheckCorrespondentAccountOnUpdate(AccountInvoiceViewModel invoiceViewModel)
        {
            AccountInvoice accountInvoice = account.AccountInvoices.FirstOrDefault(inv => inv.CorrespondentAccount == invoiceViewModel.CorrespondentAccount);
            if (accountInvoice != null && accountInvoice.Id != invoiceViewModel.Id)
                errors.Add("CorrespondentAccountAlreadyExists", resManager.GetString("CorrespondentAccountAlreadyExists"));
        }
    }
}

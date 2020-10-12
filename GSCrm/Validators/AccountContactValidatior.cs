using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class AccountContactValidatior : BaseValidator<AccountContactViewModel>
    {
        public AccountContactValidatior(ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        { }

        public override Dictionary<string, string> CreationCheck(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        public override Dictionary<string, string> UpdateCheck(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        public override Dictionary<string, string> DeleteCheck(Guid id)
        {
            AccountContact accountContact = context.AccountContacts.FirstOrDefault(i => i.Id == id);
            if (accountContact == null)
                errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));

            Account account = context.Accounts.FirstOrDefault(i => i.Id == accountContact.AccountId);
            if (account == null)
                errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));

            if (account.PrimaryContactId == accountContact.Id && account.AccountType == AccountType.Individual)
                errors.Add("PrimaryIndividualContactIsReadonly", resManager.GetString("PrimaryIndividualContactIsReadonly"));

            return errors;
        }

        /// <summary>
        /// Метод выполняет общие проверки для методов "CreationCheck" и "UpdateCheck", так как они идентичны
        /// </summary>
        /// <param name="contactViewModel"></param>
        /// <returns></returns>
        private Dictionary<string, string> CommonChecks(AccountContactViewModel contactViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckContactType(contactViewModel),
                () => CheckContactName(contactViewModel),
                () => CheckContactPhoneNumber(contactViewModel),
                () => CheckEmail(contactViewModel)
            });
            return errors;
        }

        /// <summary>
        /// Метод производит проверку тип контакта
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactType(AccountContactViewModel contactViewModel)
        {
            if (string.IsNullOrEmpty(contactViewModel.ContactType) || !Enum.TryParse(typeof(ContactType), contactViewModel.ContactType, out _))
                errors.Add("WrongContactType", resManager.GetString("WrongContactType"));
        }

        /// <summary>
        /// Метод производит проверку имени
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactName(AccountContactViewModel contactViewModel)
        {
            PersonValidator personValidator = new PersonValidator(resManager);
            personValidator.CheckPersonName(contactViewModel.FirstName, contactViewModel.LastName, contactViewModel.MiddleName, ref errors);
        }

        /// <summary>
        /// Проверка формата номера телефона
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactPhoneNumber(AccountContactViewModel contactViewModel)
        {
            if (!string.IsNullOrEmpty(contactViewModel.PhoneNumber))
            {
                PersonValidator personValidator = new PersonValidator(resManager);
                personValidator.CheckPersonPhoneNumber(contactViewModel.PhoneNumber, ref errors);
            }
        }

        /// <summary>
        /// Проверка формата почты
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckEmail(AccountContactViewModel contactViewModel)
        {
            if (!string.IsNullOrEmpty(contactViewModel.Email))
            {
                PersonValidator personValidator = new PersonValidator(resManager);
                personValidator.CheckPersonEmail(contactViewModel.Email, ref errors);
            }
        }
    }
}

using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using System.Collections.Generic;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Validators;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GSCrm.Models.Enums;

namespace GSCrm.Repository
{
    public class AccountContactRepository : BaseRepository<AccountContact, AccountContactViewModel>
    {
        public AccountContactRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(AccountContactViewModel contactViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactCreate", transaction);

        protected override bool TryCreatePrepare(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        protected override bool RespsIsCorrectOnUpdate(AccountContactViewModel contactViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactUpdate", transaction);

        protected override bool TryUpdatePrepare(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        protected override bool RespsIsCorrectOnDelete(AccountContact accountContact)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactDelete", transaction);

        protected override bool TryDeletePrepare(AccountContact accountContact)
        {
            Account account = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == accountContact.AccountId);
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (account == null)
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                },
                () => {
                    if (account.PrimaryContactId == accountContact.Id && account.AccountType == AccountType.Individual)
                        errors.Add("PrimaryIndividualContactIsReadonly", resManager.GetString("PrimaryIndividualContactIsReadonly"));
                }
            });
            return !errors.Any();
        }
        #endregion

        #region Validations
        /// <summary>
        /// Метод выполняет общие проверки для методов "CreationCheck" и "UpdateCheck", так как они идентичны
        /// </summary>
        /// <param name="contactViewModel"></param>
        /// <returns></returns>
        private bool CommonChecks(AccountContactViewModel contactViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckContactType(contactViewModel),
                () => {
                    new PersonValidator(resManager)
                        .CheckPersonName(contactViewModel.FirstName, contactViewModel.LastName, contactViewModel.MiddleName, ref errors);
                },
                () => {
                    new PersonValidator(resManager)
                        .CheckPersonPhoneNumber(contactViewModel.PhoneNumber, ref errors);
                },
                () => {
                    new PersonValidator(resManager)
                        .CheckPersonEmail(contactViewModel.Email, errors);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод производит проверку тип контакта
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactType(AccountContactViewModel contactViewModel)
        {
            if (string.IsNullOrEmpty(contactViewModel.ContactType) || !Enum.TryParse(typeof(ContactType), contactViewModel.ContactType, out object type))
            {
                errors.Add("WrongContactType", resManager.GetString("WrongContactType"));
                return;
            }
            transaction.AddParameter("ContactType", (ContactType)type);
        }
        #endregion
    }
}

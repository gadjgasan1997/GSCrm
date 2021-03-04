using System;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Validators;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class AccountContactRepository : BaseRepository<AccountContact, AccountContactViewModel>
    {
        public AccountContactRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(AccountContactViewModel contactViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactCreate");

        protected override bool TryCreatePrepare(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        protected override bool RespsIsCorrectOnUpdate(AccountContactViewModel contactViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactUpdate");

        protected override bool TryUpdatePrepare(AccountContactViewModel contactViewModel) => CommonChecks(contactViewModel);

        protected override bool RespsIsCorrectOnDelete(AccountContact accountContact)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccContactDelete");

        protected override void UpdateCacheOnDelete(AccountContact accountContact)
        {
            if (cachService.TryGetCachedEntity(currentUser, accountContact.AccountId, out Account account) &&
                cachService.TryGetCachedEntity(currentUser, accountContact.AccountId, out AccountViewModel accountViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, account);
                cachService.CacheCurrentEntity(currentUser, accountViewModel);
            }
        }

        protected override bool TryDeletePrepare(AccountContact accountContact)
        {
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            if (account.PrimaryContactId == accountContact.Id && account.AccountType == AccountType.Individual)
                errors.Add("PrimaryIndividualContactIsReadonly", resManager.GetString("PrimaryIndividualContactIsReadonly"));
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

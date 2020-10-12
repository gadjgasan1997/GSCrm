using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class AccountContactTransformer : BaseTransformer<AccountContact, AccountContactViewModel>
    {
        public AccountContactTransformer(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override AccountContactViewModel DataToViewModel(AccountContact accountContact)
        {
            Account account = context.Accounts.FirstOrDefault(i => i.Id == accountContact.AccountId);
            return new AccountContactViewModel()
            {
                Id = accountContact.Id,
                AccountId = accountContact.AccountId.ToString(),
                FirstName = accountContact.FirstName,
                LastName = accountContact.LastName,
                MiddleName = accountContact.MiddleName,
                FullName = accountContact.GetFullName(),
                ContactType = accountContact.ContactType.ToString(),
                Email = accountContact.Email,
                PhoneNumber = accountContact.PhoneNumber,
                IsPrimary = account.PrimaryContactId == accountContact.Id
            };
        }

        public override AccountContact OnModelCreate(AccountContactViewModel contactViewModel)
        {
            return new AccountContact()
            {
                AccountId = Guid.Parse(contactViewModel.AccountId),
                ContactType = (ContactType)Enum.Parse(typeof(ContactType), contactViewModel.ContactType),
                FirstName = contactViewModel.FirstName,
                LastName = contactViewModel.LastName,
                MiddleName = contactViewModel.MiddleName,
                Email = contactViewModel.Email,
                PhoneNumber = contactViewModel.PhoneNumber
            };
        }

        public override AccountContact OnModelUpdate(AccountContactViewModel contactViewModel)
        {
            AccountContact oldAccountContact = context.AccountContacts.FirstOrDefault(i => i.Id == contactViewModel.Id);
            oldAccountContact.ContactType = (ContactType)Enum.Parse(typeof(ContactType), contactViewModel.ContactType);
            oldAccountContact.FirstName = contactViewModel.FirstName;
            oldAccountContact.LastName = contactViewModel.LastName;
            oldAccountContact.MiddleName = contactViewModel.MiddleName;
            oldAccountContact.Email = contactViewModel.Email;
            oldAccountContact.PhoneNumber = contactViewModel.PhoneNumber;
            return oldAccountContact;
        }
    }
}

using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;
using GSCrm.Models.Enums;

namespace GSCrm.Mapping
{
    public class AccountContactMap : BaseMap<AccountContact, AccountContactViewModel>
    {
        public AccountContactMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        public override AccountContactViewModel DataToViewModel(AccountContact accountContact)
        {
            Account account = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == accountContact.AccountId);
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
            base.OnModelCreate(contactViewModel);
            return new AccountContact()
            {
                AccountId = Guid.Parse(contactViewModel.AccountId),
                ContactType = (ContactType)transaction.GetParameterValue("ContactType"),
                FirstName = contactViewModel.FirstName,
                LastName = contactViewModel.LastName,
                MiddleName = contactViewModel.MiddleName,
                Email = contactViewModel.Email,
                PhoneNumber = contactViewModel.PhoneNumber
            };
        }

        public override AccountContact OnModelUpdate(AccountContactViewModel contactViewModel)
        {
            AccountContact oldAccountContact = base.OnModelUpdate(contactViewModel);
            oldAccountContact.ContactType = (ContactType)transaction.GetParameterValue("ContactType");
            oldAccountContact.FirstName = contactViewModel.FirstName;
            oldAccountContact.LastName = contactViewModel.LastName;
            oldAccountContact.MiddleName = contactViewModel.MiddleName;
            oldAccountContact.Email = contactViewModel.Email;
            oldAccountContact.PhoneNumber = contactViewModel.PhoneNumber;
            return oldAccountContact;
        }
    }
}

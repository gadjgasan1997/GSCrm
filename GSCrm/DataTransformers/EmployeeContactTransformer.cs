using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class EmployeeContactTransformer : BaseTransformer<EmployeeContact, EmployeeContactViewModel>
    {
        public EmployeeContactTransformer(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override EmployeeContact OnModelCreate(EmployeeContactViewModel contactViewModel)
        {
            return new EmployeeContact()
            {
                EmployeeId = contactViewModel.EmployeeId,
                ContactType = (ContactType)Enum.Parse(typeof(ContactType), contactViewModel.ContactType),
                Email = contactViewModel.Email,
                PhoneNumber = contactViewModel.PhoneNumber
            };
        }

        public override EmployeeContact OnModelUpdate(EmployeeContactViewModel contactViewModel)
        {
            EmployeeContact oldEmployeeContact = context.EmployeeContacts.FirstOrDefault(i => i.Id == contactViewModel.Id);
            oldEmployeeContact.ContactType = (ContactType)Enum.Parse(typeof(ContactType), contactViewModel.ContactType);
            oldEmployeeContact.Email = contactViewModel.Email;
            oldEmployeeContact.PhoneNumber = contactViewModel.PhoneNumber;
            return oldEmployeeContact;
        }

        public override EmployeeContactViewModel DataToViewModel(EmployeeContact employeeContact)
        {
            return new EmployeeContactViewModel()
            {
                Id = employeeContact.Id,
                EmployeeId = employeeContact.EmployeeId,
                ContactType = employeeContact.ContactType.ToString(),
                Email = employeeContact.Email,
                PhoneNumber = employeeContact.PhoneNumber
            };
        }
    }
}

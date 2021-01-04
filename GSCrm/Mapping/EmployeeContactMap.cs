using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Models.Enums;

namespace GSCrm.Mapping
{
    public class EmployeeContactMap : BaseMap<EmployeeContact, EmployeeContactViewModel>
    {
        public EmployeeContactMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        public override EmployeeContact OnModelCreate(EmployeeContactViewModel contactViewModel)
        {
            base.OnModelCreate(contactViewModel);
            return new EmployeeContact()
            {
                EmployeeId = Guid.Parse(contactViewModel.EmployeeId),
                ContactType = (ContactType)transaction.GetParameterValue("ContactType"),
                Email = contactViewModel.Email,
                PhoneNumber = contactViewModel.PhoneNumber
            };
        }

        public override EmployeeContact OnModelUpdate(EmployeeContactViewModel contactViewModel)
        {
            EmployeeContact oldEmployeeContact = base.OnModelUpdate(contactViewModel);
            oldEmployeeContact.ContactType = (ContactType)transaction.GetParameterValue("ContactType");
            oldEmployeeContact.Email = contactViewModel.Email;
            oldEmployeeContact.PhoneNumber = contactViewModel.PhoneNumber;
            return oldEmployeeContact;
        }

        public override EmployeeContactViewModel DataToViewModel(EmployeeContact employeeContact)
        {
            return new EmployeeContactViewModel()
            {
                Id = employeeContact.Id,
                EmployeeId = employeeContact.EmployeeId.ToString(),
                ContactType = employeeContact.ContactType.ToString(),
                Email = employeeContact.Email,
                PhoneNumber = employeeContact.PhoneNumber
            };
        }
    }
}

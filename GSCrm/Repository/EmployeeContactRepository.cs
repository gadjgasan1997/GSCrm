using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Models.Enums;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class EmployeeContactRepository : BaseRepository<EmployeeContact, EmployeeContactViewModel>
    {
        public EmployeeContactRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(EmployeeContactViewModel contactViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpContactCreate", transaction);

        protected override bool TryCreatePrepare(EmployeeContactViewModel contactViewModel)
        {
            InvokeContactChecks(contactViewModel);
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(EmployeeContactViewModel contactViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpContactUpdate", transaction);

        protected override bool TryUpdatePrepare(EmployeeContactViewModel contactViewModel)
        {
            InvokeContactChecks(contactViewModel);
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(EmployeeContact employeeContact)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpContactDelete", transaction);
        #endregion

        #region Validations
        private void InvokeContactChecks(EmployeeContactViewModel contactViewModel)
            => InvokeAllChecks(new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(contactViewModel.ContactType))
                        errors.Add("NullContactType", resManager.GetString("NullContactType"));
                    if (!Enum.TryParse(typeof(ContactType), contactViewModel.ContactType, out object contactType))
                        errors.Add("WrongContactType", resManager.GetString("WrongContactType"));
                    else transaction.AddParameter("ContactType", contactType);
                },
                () => {
                    new PersonValidator(resManager).CheckPersonPhoneNumber(contactViewModel.PhoneNumber, ref errors);
                },
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(contactViewModel.Email, errors);
                }
            });
        #endregion
    }
}

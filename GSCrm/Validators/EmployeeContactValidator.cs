using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class EmployeeContactValidator : BaseValidator<EmployeeContactViewModel>
    {
        public EmployeeContactValidator(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override Dictionary<string, string> CreationCheck(EmployeeContactViewModel contactViewModel)
        {
            CommonChecks(contactViewModel);
            return errors;
        }

        public override Dictionary<string, string> UpdateCheck(EmployeeContactViewModel contactViewModel)
        {
            CommonChecks(contactViewModel);
            return errors;
        }

        private void CommonChecks(EmployeeContactViewModel contactViewModel)
        {
            InvokeAllChecks(new List<Action>()
            {
                () => CheckContactType(contactViewModel),
                () => CheckContactPhoneNumber(contactViewModel),
                () => CheckEmail(contactViewModel)
            });
        }

        /// <summary>
        /// Проверка типа контакта на недопустимые значения
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactType(EmployeeContactViewModel contactViewModel)
        {
            if (string.IsNullOrEmpty(contactViewModel.ContactType))
                errors.Add("NullContactType", resManager.GetString("NullContactType"));
            if (!Enum.TryParse(typeof(ContactType), contactViewModel.ContactType, out _))
                errors.Add("WrongContactType", resManager.GetString("WrongContactType"));
        }

        /// <summary>
        /// Проверка формата номера телефона
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckContactPhoneNumber(EmployeeContactViewModel contactViewModel)
        {
            PersonValidator personValidator = new PersonValidator(resManager);
            personValidator.CheckPersonPhoneNumber(contactViewModel.PhoneNumber, ref errors);
        }

        /// <summary>
        /// Проверка формата почты
        /// </summary>
        /// <param name="contactViewModel"></param>
        private void CheckEmail(EmployeeContactViewModel contactViewModel)
        {
            PersonValidator personValidator = new PersonValidator(resManager);
            personValidator.CheckPersonEmail(contactViewModel.Email, ref errors);
        }
    }
}

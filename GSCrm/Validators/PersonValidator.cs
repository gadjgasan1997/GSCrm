using GSCrm.Localization;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GSCrm.Validators
{
    public class PersonValidator
    {
        private readonly ResManager resManager;
        private const int FIRST_NAME_MIN_LENGTH = 2;
        private const int FIRST_NAME_MAX_LENGTH = 300;
        private const int LAST_NAME_MIN_LENGTH = 2;
        private const int LAST_NAME_MAX_LENGTH = 300;
        private const int MID_NAME_MAX_LENGTH = 300;
        public PersonValidator(ResManager resManager)
        {
            this.resManager = resManager;
        }

        /// <summary>
        /// Проверяет фамилию, имя и отчество
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="middleName"></param>
        /// <param name="errors"></param>
        public void CheckPersonName(string firstName, string lastName, string middleName, ref Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < FIRST_NAME_MIN_LENGTH || firstName.Length > FIRST_NAME_MAX_LENGTH)
                errors.Add("FirstNameLength", resManager.GetString("FirstNameLength"));
            if (string.IsNullOrEmpty(lastName) || lastName.Length < LAST_NAME_MIN_LENGTH || lastName.Length > LAST_NAME_MAX_LENGTH)
                errors.Add("LastNameLength", resManager.GetString("LastNameLength"));
            if (!string.IsNullOrEmpty(middleName) && middleName.Length > MID_NAME_MAX_LENGTH)
                errors.Add("MiddleNameLength", resManager.GetString("MiddleNameLength"));
        }

        /// <summary>
        /// Проверка формата номера телефона
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="errors"></param>
        public void CheckPersonPhoneNumber(string phoneNumber, ref Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(phoneNumber) || Regex.IsMatch(phoneNumber, @"[^0-9()-]+$"))
                errors.Add("WrongPhoneFormat", resManager.GetString("WrongPhoneFormat"));
        }

        /// <summary>
        /// Проверка формата почты
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        public void CheckPersonEmail(string email, ref Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(email))
                errors.Add("WrongEmailFormat", resManager.GetString("WrongEmailFormat"));
            else
            {
                try
                {
                    new MailAddress(email);
                }
                catch (FormatException)
                {
                    errors.Add("WrongEmailFormat", resManager.GetString("WrongEmailFormat"));
                }
            }
        }
    }
}

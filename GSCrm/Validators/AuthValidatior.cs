using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    /// <summary>
    /// Содержит методы для валидации данных связанных с авторизацией и управлением пользователями
    /// </summary>
    public class AuthValidatior
    {
        private readonly UserManager<User> userManager;
        private readonly ResManager resManager;
        private const int USER_NAME_MIN_LENGTH = 4;
        private const int USER_NAME_MAX_LENGTH = 35;

        public Dictionary<string, string> errors;

        public AuthValidatior(UserManager<User> userManager, ResManager resManager)
        {
            errors = new Dictionary<string, string>();
            this.userManager = userManager;
            this.resManager = resManager;
        }
        
        /// <summary>
        /// Проверка данных, введенных польователем при регистрации
        /// </summary>
        /// <param name="model"></param>
        public Dictionary<string, string> RegistrationCheck(UserModel model)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckUserNameLength(model),
                () => CheckUserNameNotExists(model),
                () => CheckEmail(model),
                () => CheckNewPassword(model)
            });
            return errors;
        }

        /// <summary>
        /// Проверка данных, введенных пользователем при авторизации
        /// </summary>
        /// <param name="model"></param>
        public Dictionary<string, string> LoginCheck(UserModel model)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckUserNameExists(model),
                () => CheckPasswordCorrect(model)
            });
            return errors;
        }

        /// <summary>
        /// Проверка данных при указании почты для сброса пароля
        /// </summary>
        /// <param name="model"></param>
        public Dictionary<string, string> SpecifyEmailCheck(UserModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                errors.Add("EmailIsNull", resManager.GetString("EmailIsNull"));
            else if (userManager.FindByEmailAsync(model.Email).Result == null && userManager.FindByNameAsync(model.Email).Result == null)
                errors.Add("EmailNotExists", resManager.GetString("EmailNotExists"));
            return errors;
        }

        /// <summary>
        /// Проверка данных при сбросе пароля
        /// </summary>
        /// <param name="model"></param>
        public Dictionary<string, string> ResetPasswordCheck(UserModel model)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckUserNameLength(model),
                () => CheckUserNameExists(model),
                () => CheckNewPassword(model),
                () => CheckPasswordNotCompare(model)
            });
            return errors;
        }

        /// <summary>
        /// Проверка имени пользователя на длину
        /// </summary>
        /// <param name="model"></param>
        public void CheckUserNameLength(UserModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || model.UserName.Length < USER_NAME_MIN_LENGTH || model.UserName.Length > USER_NAME_MAX_LENGTH)
                errors.Add("UserNameLength", resManager.GetString("UserNameLength"));
        }

        /// <summary>
        /// Проверка на отсутствие пользователя с таким же именем в бд
        /// </summary>
        /// <param name="model"></param>
        private void CheckUserNameNotExists(UserModel model)
        {
            if (model.UserName != null && userManager.FindByNameAsync(model.UserName).Result != null)
                errors.Add("UserNameAlreadyExists", resManager.GetString("UserNameAlreadyExists"));
        }

        /// <summary>
        /// Проверка на наличие пользователя с таким именем в бд
        /// </summary>
        /// <param name="model"></param>
        private void CheckUserNameExists(UserModel model)
        {
            if (model.UserName == null || userManager.FindByNameAsync(model.UserName).Result == null)
                errors.Add("UserNameNotExists", resManager.GetString("UserNameNotExists"));
        }

        /// <summary>
        /// Проверка почты на длину и отсутствие пользователя с такой же почтой в бд
        /// </summary>
        /// <param name="model"></param>
        private void CheckEmail(UserModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                errors.Add("EmailIsNull", resManager.GetString("EmailIsNull"));
            else if (userManager.FindByEmailAsync(model.Email).Result != null)
                errors.Add("EmailAlreadyExists", resManager.GetString("EmailAlreadyExists"));
        }

        /// <summary>
        /// Проверка нового пароля на длину и наличие требуемых символов
        /// </summary>
        /// <param name="model"></param>
        private void CheckNewPassword(UserModel model)
        {
            PasswordValidator passwordValidator = new PasswordValidator(resManager);
            IdentityResult passwordValidation = passwordValidator.ValidateAsync(userManager, userManager.FindByNameAsync(model.UserName).Result, model.Password).Result;
            if (!passwordValidation.Succeeded)
                passwordValidation.Errors.ToList().ForEach(error => errors.Add(error.Code, error.Description));
            if (!errors.Any())
            {
                IdentityResult passwordConfirmation = passwordValidator.CheckPasswordConfirmation(model).Result;
                if (!passwordConfirmation.Succeeded)
                    passwordConfirmation.Errors.ToList().ForEach(error => errors.Add(error.Code, error.Description));
            }
        }

        /// <summary>
        /// Проверка правильности пароля при авторизации
        /// </summary>
        /// <param name="model"></param>
        private void CheckPasswordCorrect(UserModel model)
        {
            User user = userManager.FindByNameAsync(model.UserName).Result;
            if (model.Password == null || !userManager.CheckPasswordAsync(user, model.Password).Result)
                errors.Add("WrongPassword", resManager.GetString("WrongPassword"));
        }

        /// <summary>
        /// Проверка что новый пароль не совпадает с текущим
        /// </summary>
        /// <param name="model"></param>
        private void CheckPasswordNotCompare(UserModel model)
        {
            if (model.OldPassword == model.Password)
                errors.Add("PasswordAlreadyUsed", resManager.GetString("PasswordAlreadyUsed"));
        }
    }
}

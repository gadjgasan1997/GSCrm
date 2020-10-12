using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Validators
{
    /// <summary>
    /// Кастомная проверка пароля
    /// </summary>
    public class PasswordValidator : IPasswordValidator<User>
    {
        private readonly ResManager resManager;
        private const int PASSWORD_MIN_LENGTH = 8;
        private const int PASSWORD_MAX_LENGTH = 35;
        public PasswordValidator(ResManager resManager)
        {
            this.resManager = resManager;
        }

        /// <summary>
        /// Валдиация пароля, метод из интерфейса IPasswordValidator
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(password) || password.Length < PASSWORD_MIN_LENGTH || password.Length > PASSWORD_MAX_LENGTH)
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordLength",
                    Description = resManager.GetString("PasswordLength")
                });
            }
            else
            {
                if (!Regex.IsMatch(password, @"[a-z]") && !Regex.IsMatch(password, @"[A-Z]"))
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "PasswordNotContainsLetters",
                        Description = resManager.GetString("PasswordNotContainsLetters")
                    });
                }
                if (!Regex.IsMatch(password, @"[0-9]"))
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "PasswordNotContainsDigits",
                        Description = resManager.GetString("PasswordNotContainsDigits")
                    });
                }
            }
            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }

        /// <summary>
        /// Метод проверяет длину пароля из поля для повторения пароля
        /// и его совпадение с паролем из поля "пароль"
        /// </summary>
        /// <param name="model"></param>
        public Task<IdentityResult> CheckPasswordConfirmation(UserModel model)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(model.ConfirmPassword) || model.ConfirmPassword.Length < PASSWORD_MIN_LENGTH || model.ConfirmPassword.Length > PASSWORD_MAX_LENGTH)
            {
                errors.Add(new IdentityError()
                {
                    Code = "ConfirmPasswordLength",
                    Description = resManager.GetString("ConfirmPasswordLength")
                });
            }
            else if (model.Password != model.ConfirmPassword)
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordsNotEquals",
                    Description = resManager.GetString("PasswordsNotEquals")
                });
            }
            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}

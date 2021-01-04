using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using GSCrm.Notifications.Factories.NotFactories;
using GSCrm.Notifications.Params;
using GSCrm.Transactions;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GSCrm.Repository
{
    public class AuthRepository
    {
        #region Delcarations
        private readonly IServiceProvider serviceProvider;
        private readonly IResManager resManager;
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;
        /// <summary>
        /// Сервис транзакций
        /// </summary>
        protected readonly ITransactionFactory<UserViewModel> transactionFactory;
        private ITransaction transaction;
        /// <summary>
        /// Менеджер для регистрации пользователей
        /// </summary>
        private readonly UserManager<User> userManager;
        /// <summary>
        /// Менеджер для авторизации пользователей
        /// </summary>
        private readonly SignInManager<User> signInManager;
        /// <summary>
        /// Список ошибок
        /// </summary>
        public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// Минимальная длина имени пользователя
        /// </summary>
        private const int USER_NAME_MIN_LENGTH = 4;
        /// <summary>
        /// Максимальная длина имени пользователя
        /// </summary>
        private const int USER_NAME_MAX_LENGTH = 35;
        #endregion

        #region Constructs
        public AuthRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            userManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
            signInManager = serviceProvider.GetService(typeof(SignInManager<User>)) as SignInManager<User>;
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;
            this.context = context;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            httpContext = userContextServices.HttpContext;
            ITFFactory TFFactory = serviceProvider.GetService(typeof(ITFFactory)) as ITFFactory;
            transactionFactory = TFFactory.GetTransactionFactory<UserViewModel>(serviceProvider, context);
        }
        #endregion

        #region Validations
        public bool TrySignupValidate(UserViewModel userModel)
        {
            InvokeIntermittinActions(Errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(userModel.UserName) || userModel.UserName.Length < USER_NAME_MIN_LENGTH || userModel.UserName.Length > USER_NAME_MAX_LENGTH)
                        Errors.Add("UserNameLength", resManager.GetString("UserNameLength"));
                },
                () => {
                    if (userModel.UserName != null && userManager.FindByNameAsync(userModel.UserName).Result != null)
                        Errors.Add("UserNameAlreadyExists", resManager.GetString("UserNameAlreadyExists"));
                },
                () => {
                    if (string.IsNullOrEmpty(userModel.Email))
                        Errors.Add("EmailIsNull", resManager.GetString("EmailIsNull"));
                },
                () => {
                    if (userManager.FindByEmailAsync(userModel.Email).Result != null)
                        Errors.Add("EmailAlreadyExists", resManager.GetString("EmailAlreadyExists"));
                },
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(userModel.Email, Errors);
                },
                () => CheckNewPassword(userModel)
            });
            return !Errors.Any();
        }

        private bool TryLoginValidate(UserViewModel userModel)
        {
            InvokeIntermittinActions(Errors, new List<Action>()
            {
                () => CheckUserNameExists(userModel),
                () => CheckPasswordCorrect(userModel)
            });
            return !Errors.Any();
        }

        /// <summary>
        /// Проверка данных при указании почты для сброса пароля
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        private bool TryResetPasswordSendEmailValidate(UserViewModel userModel)
        {
            InvokeIntermittinActions(Errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(userModel.Email))
                        Errors.Add("EmailIsNull", resManager.GetString("EmailIsNull"));
                },
                () => {
                    if (userManager.FindByEmailAsync(userModel.Email).Result == null && userManager.FindByNameAsync(userModel.Email).Result == null)
                        Errors.Add("EmailNotExists", resManager.GetString("EmailNotExists"));
                }
            });
            return !Errors.Any();
        }

        /// <summary>
        /// Проверка данных при сбросе пароля
        /// </summary>
        /// <param name="model"></param>
        private bool TryResetPasswordValidate(UserViewModel model)
        {
            InvokeIntermittinActions(Errors, new List<Action>()
            {
                () => CheckUserNameExists(model),
                () => {
                    if (string.IsNullOrEmpty(model.OldPassword))
                        Errors.Add("OldPasswordLength", resManager.GetString("OldPasswordLength"));
                },
                () => {
                    bool isPassCorrect = userManager.CheckPasswordAsync((User)transaction.GetParameterValue("UserAccount"), model.OldPassword).Result;
                    if (!isPassCorrect)
                        Errors.Add("OldPasswordWrong", resManager.GetString("OldPasswordWrong"));
                },
                () => CheckNewPassword(model),
                () => {
                    if (model.OldPassword == model.Password)
                        Errors.Add("PasswordAlreadyUsed", resManager.GetString("PasswordAlreadyUsed"));
                }
            });
            return !Errors.Any();
        }

        /// <summary>
        /// Проверка на наличие пользователя с таким именем в бд
        /// </summary>
        /// <param name="model"></param>
        private void CheckUserNameExists(UserViewModel model)
        {
            if (model.UserName != null)
            {
                User userAccount = userManager.FindByNameAsync(model.UserName).Result;
                if (userAccount != null)
                {
                    transaction.AddParameter("UserAccount", userAccount);
                    return;
                }
            }
            Errors.Add("UserNameNotExists", resManager.GetString("UserNameNotExists"));
        }

        /// <summary>
        /// Проверка нового пароля на длину и наличие требуемых символов
        /// </summary>
        /// <param name="model"></param>
        private void CheckNewPassword(UserViewModel model)
        {
            PasswordValidator passwordValidator = new PasswordValidator(resManager);
            IdentityResult passwordValidation = passwordValidator.ValidateAsync(userManager, userManager.FindByNameAsync(model.UserName).Result, model.Password).Result;
            if (!passwordValidation.Succeeded)
                passwordValidation.Errors.ToList().ForEach(error => Errors.Add(error.Code, error.Description));
            if (!Errors.Any())
            {
                IdentityResult passwordConfirmation = passwordValidator.CheckPasswordConfirmation(model).Result;
                if (!passwordConfirmation.Succeeded)
                    passwordConfirmation.Errors.ToList().ForEach(error => Errors.Add(error.Code, error.Description));
            }
        }

        /// <summary>
        /// Проверка правильности пароля при авторизации
        /// </summary>
        /// <param name="model"></param>
        private void CheckPasswordCorrect(UserViewModel model)
        {
            User user = userManager.FindByNameAsync(model.UserName).Result;
            if (model.Password == null || !userManager.CheckPasswordAsync(user, model.Password).Result)
                Errors.Add("WrongPassword", resManager.GetString("WrongPassword"));
        }
        #endregion

        #region Registration
        public async Task<bool> TrySignup(UserViewModel userModel, IUrlHelper urlHelper, ModelStateDictionary modelState)
        {
            transaction = transactionFactory.Create(OperationType.Register, userModel);

            // Валидация при регистрации пользователя
            if (TrySignupValidate(userModel))
            {
                userModel.EmailConfirmed = false;
                User user = await TryCreateUser(userModel, Errors, transaction);
                if (user != null)
                {
                    // Попытка сделать коммит
                    if (transactionFactory.TryCommit(transaction, Errors))
                    {
                        // Создание и отправка сообщения с подтверждением регистрации
                        string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        string confirmEmailUrl = urlHelper.Action("ConfirmEmail", AUTH, new { userId = user.Id, token }, httpContext.Request.Scheme);
                        UserRegisterParams userRegisterParams = new UserRegisterParams()
                        {
                            Token = token,
                            ConfirmEmailUrl = confirmEmailUrl
                        };
                        await new UserRegisterNotFacory(serviceProvider, context, userRegisterParams).SendAsync(user);

                        // Закрытие транзакции и выход
                        transactionFactory.Close(transaction, TransactionStatus.Success);
                        return true;
                    }
                }
            }

            // Добавление ошибок в модель и закрытие транзакции с ошибкой
            transactionFactory.Close(transaction, TransactionStatus.Error);
            foreach (KeyValuePair<string, string> error in Errors)
                modelState.AddModelError(error.Key, error.Value);
            return false;
        }
        #endregion

        #region Login
        public async Task<bool> TryLogin(UserViewModel userModel, ModelStateDictionary modelState)
        {
            transaction = transactionFactory.Create(OperationType.Login, userModel);

            // Валидация при авторизации пользователя
            if (TryLoginValidate(userModel))
            {
                User user = await userManager.FindByNameAsync(userModel.UserName);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        await signInManager.SignOutAsync();
                        SignInResult signInResult = await signInManager.PasswordSignInAsync(user, userModel.Password, true, true);
                        if (signInResult.Succeeded)
                        {
                            transactionFactory.Close(transaction, TransactionStatus.Success);
                            return true;
                        }
                    }
                    else modelState.AddModelError("ConfirmYourEmail", resManager.GetString("ConfirmYourEmail"));
                }
            }

            // Добавление ошибок в модель и закрытие транзакции с ошибкой
            transactionFactory.Close(transaction, TransactionStatus.Error);
            foreach (KeyValuePair<string, string> error in Errors)
                modelState.AddModelError(error.Key, error.Value);
            return false;
        }
        #endregion

        #region Reset Password
        public async Task<bool> TrySendResetPasswordEmail(UserViewModel userModel, IUrlHelper urlHelper, ModelStateDictionary modelState)
        {
            transaction = transactionFactory.Create(OperationType.ResetPasswordSpecifyEmail, userModel);

            // Валидация при указании почты
            if (TryResetPasswordSendEmailValidate(userModel))
            {
                User user = await userManager.FindByEmailAsync(userModel.Email);
                if (user != null)
                {
                    UserResetPasswordParams resetPasswordParams = new UserResetPasswordParams()
                    {
                        ResetPasswordUrl = urlHelper.Action("ResetPassword", AUTH, new { }, httpContext.Request.Scheme)
                    };
                    await new UserResetPasswordNotFactory(serviceProvider, context, resetPasswordParams).SendAsync(user);
                    transactionFactory.Close(transaction, TransactionStatus.Success);
                    return true;
                }
            }

            // Добавление ошибок в модель и закрытие транзакции с ошибкой
            transactionFactory.Close(transaction, TransactionStatus.Error);
            foreach (KeyValuePair<string, string> error in Errors)
                modelState.AddModelError(error.Key, error.Value);
            return false;
        }

        public async Task<bool> TryResetPassword(UserViewModel userModel, ModelStateDictionary modelState)
        {
            transaction = transactionFactory.Create(OperationType.ResetPassword, userModel);

            // Валидация при сбросе пароля
            if (TryResetPasswordValidate(userModel))
            {
                User user = await userManager.FindByNameAsync(userModel.UserName);
                if (user != null)
                {
                    await userManager.ChangePasswordAsync(user, userModel.OldPassword, userModel.Password);
                    transactionFactory.Close(transaction, TransactionStatus.Success);
                    return true;
                }
            }

            // Добавление ошибок в модель и закрытие транзакции с ошибкой
            transactionFactory.Close(transaction, TransactionStatus.Error);
            foreach (KeyValuePair<string, string> error in Errors)
                modelState.AddModelError(error.Key, error.Value);
            return false;
        }
        #endregion

        #region Other Methods
        public async Task<User> TryCreateUser(UserViewModel userModel, Dictionary<string, string> errors, ITransaction transaction)
        {
            User user = new User()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                EmailConfirmed = userModel.EmailConfirmed
            };
            IdentityResult identityResult = await userManager.CreateAsync(user, userModel.Password);
            if (identityResult.Succeeded)
            {
                OnUserSucceededCreation(user, transaction);
                return user;
            }
            else
            {
                foreach (IdentityError error in identityResult.Errors)
                    errors.Add(error.Code, GetDescriptionFromIdentityCode(error.Code));
                return null;
            }
        }

        private string GetDescriptionFromIdentityCode(string identityCode)
            => identityCode switch
            {
                "InvalidUserName" => resManager.GetString("UserNameWrong"),
                _ => resManager.GetString("UnhandledException")
            };

        /// <summary>
        /// Метод обрабатывает успешное создание пользователя
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="transaction"></param>
        private void OnUserSucceededCreation(User newUser, ITransaction transaction)
        {
            // Добавление настроек уведомлений
            transaction.AddChange(new UserNotificationsSetting()
            {
                Id = Guid.NewGuid(),
                OrgInvoiceNot = true,
                TOrgInvoiceNot = NotificationTarget.Inbox,
                UserId = newUser.Id,
                User = newUser
            }, EntityState.Added);
        }
        #endregion
    }
}

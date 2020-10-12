using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Services;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    //[Route(AUTH)]
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ResManager resManager;
        public AuthController(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, ResManager resManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.resManager = resManager;
        }

        /// <summary>
        /// Авторизованного пользователя это действие переводит на домашнюю страницу
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View("Index");
        }

        /// <summary>
        /// Метод вначале проверяет данные вызовом метода RegistrationCheck, затем высылает пользователю на почту письмо с подтверждением регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Signup(UserModel model)
        {
            AuthValidatior authValidation = new AuthValidatior(userManager, resManager);
            Dictionary<string, string> errors = authValidation.RegistrationCheck(model);
            if (!errors.Any())
            {
                User user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = false
                };
                IdentityResult identityResult = await userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmEmailUrl = Url.Action("ConfirmEmail", AUTH, new { userId = user.Id, token }, protocol: HttpContext.Request.Scheme);
                    EmailSender emailSender = new EmailSender(configuration, resManager);
                    await emailSender.SendRegisterConfirmationEmail(model, confirmEmailUrl);
                    return Json(Url.Action("ConfirmEmailMessage", AUTH));
                }
                else
                {
                    foreach (IdentityError error in identityResult.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                }
            }
            errors.ToList().ForEach(error => ModelState.AddModelError(error.Key, error.Value));
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Представление с сообщением, что необходимо подтвердить почту
        /// </summary>
        /// <returns></returns>
        [HttpGet("ConfirmEmailMessage")]
        public ViewResult ConfirmEmailMessage() => View("ConfirmEmailMessage");

        /// <summary>
        /// Метод подтверждения электронной почты
        /// Если токен подтверждения не передан или пользователь не найден, осуществится переход на страницу с ошибкой
        /// Тем самым гарантируя, что подтвердить почту можно только через выданную ссылку
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="token">Токен подтверждения почты</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return View("Error");

            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return View("Error");

            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return RedirectToAction("EmailConfirmed", "Auth");
            return View("Error");
        }

        /// <summary>
        /// Представление с сообщением, что почта успешно подтверждена
        /// </summary>
        /// <returns></returns>
        [HttpGet("EmailConfirmed")]
        public ViewResult EmailConfirmed() => View("EmailConfirmed");

        /// <summary>
        /// Метод вначале проверяет данные вызовом метода LoginCheck, затем выполняет авторизацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(UserModel model)
        {
            AuthValidatior authValidation = new AuthValidatior(userManager, resManager);
            Dictionary<string, string> errors = authValidation.LoginCheck(model);
            if (!errors.Any())
            {
                User user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        await signInManager.SignOutAsync();
                        SignInResult signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
                        if (signInResult.Succeeded)
                            return Json(Url.Action("Index", "Home"));
                    }
                    else ModelState.AddModelError("ConfirmYourEmail", resManager.GetString("ConfirmYourEmail"));
                }
            }
            errors.ToList().ForEach(error => ModelState.AddModelError(error.Key, error.Value));
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Представление с указанием почты для сброса пароля
        /// </summary>
        /// <returns></returns>
        [HttpGet("ResetPasswordSpecifyEmail")]
        public ViewResult ResetPasswordSpecifyEmail() => View("ResetPasswordSpecifyEmail");

        /// <summary>
        /// Представление с информацией о том, что для сброса пароля необходимо проверть почту
        /// </summary>
        /// <returns></returns>
        [HttpGet("ResetPasswordMessage")]
        public ViewResult ResetPasswordMessage() => View("ResetPasswordMessage");

        /// <summary>
        /// Метод проверяет введенный пользователем email и отправляет на него ссылку для сброса пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPasswordSendEmail(UserModel model)
        {
            AuthValidatior authValidation = new AuthValidatior(userManager, resManager);
            Dictionary<string, string> errors = authValidation.SpecifyEmailCheck(model);
            if (!errors.Any())
            {
                User user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    EmailSender emailSender = new EmailSender(configuration, resManager);
                    string resetPasswordUrl = Url.Action("ResetPassword", AUTH, new { }, HttpContext.Request.Scheme);
                    await emailSender.SendResetPasswordEmail(model, resetPasswordUrl);
                    return RedirectToAction("ResetPasswordMessage", AUTH);
                }
            }
            errors.ToList().ForEach(error => ModelState.AddModelError(error.Key, error.Value));
            return View("ResetPasswordSpecifyEmail", ModelState);
        }

        /// <summary>
        /// Представление для сброса пароля
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ResetPassword() => View("ResetPassword");

        /// <summary>
        /// Выполняется проверка данных, введенных пользователем, затем сбрасывается пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserModel model)
        {
            AuthValidatior authValidation = new AuthValidatior(userManager, resManager);
            Dictionary<string, string> errors = authValidation.ResetPasswordCheck(model);
            if (!errors.Any())
            {
                User user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                    return RedirectToAction("ResetPasswordSuccess");
                }
            }
            errors.ToList().ForEach(error => ModelState.AddModelError(error.Key, error.Value));
            return View("ResetPassword", ModelState);
        }

        /// <summary>
        /// Страница с информацией о том, что пароль был успешно сброшен
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ResetPasswordSuccess() => View("ResetPasswordSuccess");

        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", AUTH);
        }
    }
}

using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GSCrm.Repository;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using GSCrm.Data;

namespace GSCrm.Controllers
{
    //[Route(AUTH)]
    public class AuthController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public AuthController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            userManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
            signInManager = serviceProvider.GetService(typeof(SignInManager<User>)) as SignInManager<User>;
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
        public async Task<IActionResult> Signup(UserViewModel model)
        {
            ModelStateDictionary modelState = ModelState;
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            if (await authRepository.TrySignup(model, Url, modelState))
                return Json(Url.Action("ConfirmEmailMessage", AUTH));
            else return BadRequest(ModelState);
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
        public async Task<IActionResult> Login(UserViewModel model)
        {
            ModelStateDictionary modelState = ModelState;
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            if (await authRepository.TryLogin(model, modelState))
                return Json(Url.Action("Index", "Home"));
            else return BadRequest(ModelState);
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
        public async Task<IActionResult> ResetPasswordSendEmail(UserViewModel model)
        {
            ModelStateDictionary modelState = ModelState;
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            if (await authRepository.TrySendResetPasswordEmail(model, Url, modelState))
                return View("ResetPasswordMessage", model);
            else return View("ResetPasswordSpecifyEmail", model);
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
        public async Task<IActionResult> ResetPassword(UserViewModel model)
        {
            ModelStateDictionary modelState = ModelState;
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            if (await authRepository.TryResetPassword(model, modelState))
                return View("ResetPasswordSuccess", model);
            else return View("ResetPassword", model);
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

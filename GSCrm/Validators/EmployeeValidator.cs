using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class EmployeeValidator : BaseValidator<EmployeeViewModel>
    {
        private Organization currentOrganization;
        private readonly UserManager<User> userManager;
        public EmployeeValidator(UserManager<User> userManager, ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Проверка работника при создании
        /// </summary>
        /// <param name="divisionViewModel"></param>
        /// <returns></returns>
        public override Dictionary<string, string> CreationCheck(EmployeeViewModel employeeViewModel)
        {
            SetUpCurrentOrganization(employeeViewModel);
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckEmployeeAccount(employeeViewModel),
                () => CheckEmployeeName(employeeViewModel),
                () => CheckDivisionLength(employeeViewModel),
            });

            if (TryGetDivision(employeeViewModel, out Division division))
            {
                InvokeIntermittingChecks(errors, new List<Action>()
                {
                    () => CheckPositionLength(employeeViewModel),
                    () => CheckPositionExists(employeeViewModel, division)
                });
            }
            return errors;
        }

        /// <summary>
        /// Проверка сотрудника при обновлении
        /// </summary>
        /// <param name="divisionViewModel"></param>
        /// <returns></returns>
        public override Dictionary<string, string> UpdateCheck(EmployeeViewModel employeeViewModel)
        {
            CheckEmployeeName(employeeViewModel);
            return errors;
        }

        /// <summary>
        /// Проверка при смене подразделения сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        public Dictionary<string, string> ChangeDivisionCheck(EmployeeViewModel employeeViewModel)
        {
            SetUpCurrentOrganization(employeeViewModel);
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckDivisionLength(employeeViewModel),
                () => CheckPositionLength(employeeViewModel),
                () => CheckDivisionForSelected(employeeViewModel),
            });

            if (TryGetDivision(employeeViewModel, out Division division))
                CheckPositionExists(employeeViewModel, division);
            return errors;
        }

        /// <summary>
        /// Проверка модели при разблокировке сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        public Dictionary<string, string> UnlockValidateCheck(EmployeeViewModel employeeViewModel)
        {
            SetUpCurrentOrganization(employeeViewModel);
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckDivisionLength(employeeViewModel),
                () => CheckPositionLength(employeeViewModel)
            });

            if (TryGetDivision(employeeViewModel, out Division division))
                CheckPositionExists(employeeViewModel, division);
            return errors;
        }

        /// <summary>
        /// Устанаваливает текущую организацию
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void SetUpCurrentOrganization(EmployeeViewModel employeeViewModel) => currentOrganization = employeeViewModel.GetOrganization(context);

        /// <summary>
        /// Проверяет фамилию, имя и отчество
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckEmployeeName(EmployeeViewModel employeeViewModel)
            => new PersonValidator(resManager).CheckPersonName(employeeViewModel.FirstName, employeeViewModel.LastName, employeeViewModel.MiddleName, ref errors);

        /// <summary>
        /// Проверяет прикрепленный к работнику или вновь созданный аккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            if (employeeViewModel.UserAccountExists)
                CheckExistsEmployeeAccount(employeeViewModel);
            else CheckNewEmployeeAccount(employeeViewModel);

        }

        /// <summary>
        /// Проверяет на существование и занятость акккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckExistsEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            // Проверка имени пользователя
            if (string.IsNullOrEmpty(employeeViewModel.UserName))
            {
                errors.Add("UserNameIsNull", resManager.GetString("UserNameIsNull"));
                return;
            }

            // Проверка, что такая учетная запись существует в этой организации
            User user = context.Users.FirstOrDefault(n => n.UserName == employeeViewModel.UserName);
            if (user == null)
            {
                errors.Add("UserNotExists", resManager.GetString("UserNotExists"));
                return;
            }

            // Проверка, что учетная запись не занята другим сотрудником
            Employee employeeWithSameAccount = context.GetOrgEmployees(employeeViewModel.OrganizationId).FirstOrDefault(i => i.UserId == Guid.Parse(user.Id));
            if (employeeWithSameAccount != null)
            {
                errors.Add("UserAccountIsBusy", $"{resManager.GetString("UserAccountIsBusy")}: {employeeWithSameAccount.GetIntialsFullName()}");
                return;
            }
        }

        /// <summary>
        /// Проверяет новый созданный акккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckNewEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            AuthValidatior authValidatior = new AuthValidatior(userManager, resManager);
            Dictionary<string, string> registerErrors = authValidatior.RegistrationCheck(new UserModel()
            {
                UserName = employeeViewModel.UserName,
                Email = employeeViewModel.Email,
                Password = employeeViewModel.Password,
                ConfirmPassword = employeeViewModel.ConfirmPassword
            });

            // Если проверка модели регистрации пройдена
            if (registerErrors.Any())
                registerErrors.ToList().ForEach(error => errors.Add(error.Key, error.Value));
        }

        /// <summary>
        /// Проверка на заполненность подразделения
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckDivisionLength(EmployeeViewModel employeeViewModel)
        {
            if (string.IsNullOrEmpty(employeeViewModel.DivisionName))
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }
        
        /// <summary>
        /// Проверка на заполненность основной должности
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckPositionLength(EmployeeViewModel employeeViewModel)
        {
            if (string.IsNullOrEmpty(employeeViewModel.PrimaryPositionName))
                errors.Add("PositionNameLength", resManager.GetString("PositionNameLength"));
        }

        /// <summary>
        /// Метод проверяет наличие подразделения с таким именем, и, если оно есть, возвращает его
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private bool TryGetDivision(EmployeeViewModel employeeViewModel, out Division division)
        {
            division = currentOrganization.Divisions.FirstOrDefault(n => n.Name == employeeViewModel.DivisionName);
            if (division != null) return true;

            errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
            return false;
        }

        /// <summary>
        /// Метод проверяет наличие должности в поданом на вход подразделении
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="division"></param>
        private void CheckPositionExists(EmployeeViewModel employeeViewModel, Division division)
        {
            Position primaryPosition = division.Positions.FirstOrDefault(n => n.Name == employeeViewModel.PrimaryPositionName);
            if (primaryPosition == null)
                errors.Add("PositionNotExists", resManager.GetString("PositionNotExists"));
        }

        /// <summary>
        /// Метод проверяет, что пользователь выбрал другое подразделение для сотрудника при его изменении
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="division"></param>
        private void CheckDivisionForSelected(EmployeeViewModel employeeViewModel)
        {
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == employeeViewModel.Id);
            Division division = currentOrganization.Divisions.FirstOrDefault(i => i.Id == employee.DivisionId);
            if (division.Name == employeeViewModel.DivisionName)
                errors.Add("ThisEmployeeDivisionIsAlreadySelect", resManager.GetString("ThisEmployeeDivisionIsAlreadySelect"));
        }
    }
}

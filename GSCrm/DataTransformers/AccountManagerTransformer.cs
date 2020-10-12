using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class AccountManagerTransformer : BaseTransformer<AccountManager, AccountManagerViewModel>
    {
        public AccountManagerTransformer(ApplicationDbContext context, ResManager resManager) : base (context, resManager)
        { }

        public override AccountManagerViewModel DataToViewModel(AccountManager accountManager)
        {
            // Получение всех необходимых параметров
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == accountManager.ManagerId);
            Account account = context.Accounts.FirstOrDefault(i => i.Id == accountManager.AccountId);
            Position position = employee.PrimaryPositionId == null ? null : context.Positions.FirstOrDefault(i => i.Id == employee.PrimaryPositionId);
            bool isPrimary = accountManager.Id == account.PrimaryManagerId;
            bool isLock = position == null;
            string positionName = isLock ? string.Empty : position.Name;
            Func<EmployeeContact, bool> predicate = empCon => empCon.EmployeeId == employee.Id && empCon.ContactType == ContactType.Work;
            EmployeeContact employeeContact = context.EmployeeContacts.FirstOrDefault(predicate);
            string phoneNumber = employeeContact?.PhoneNumber;

            // Возврат результата
            return new AccountManagerViewModel()
            {
                Id = accountManager.Id,
                EmployeeId = employee.Id,
                InitialName = employee.GetIntialsFullName(),
                IsPrimary = isPrimary,
                IsLock = isLock,
                PositionName = positionName,
                PhoneNumber = phoneNumber
            };
        }
    }
}

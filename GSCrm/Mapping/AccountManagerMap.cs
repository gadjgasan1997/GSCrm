using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping
{
    public class AccountManagerMap : BaseMap<AccountManager, AccountManagerViewModel>
    {
        public AccountManagerMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context)
        { }

        public override AccountManagerViewModel DataToViewModel(AccountManager accountManager)
        {
            // Получение всех необходимых параметров
            Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == accountManager.ManagerId);
            Account account = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == accountManager.AccountId);
            Position position = employee.PrimaryPositionId == null ? null : context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employee.PrimaryPositionId);
            bool isPrimary = accountManager.Id == account.PrimaryManagerId;
            bool isLock = position == null;
            string positionName = isLock ? string.Empty : position.Name;
            Func<EmployeeContact, bool> predicate = empCon => empCon.EmployeeId == employee.Id && empCon.ContactType == ContactType.Work;
            EmployeeContact employeeContact = context.EmployeeContacts.AsNoTracking().FirstOrDefault(predicate);
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

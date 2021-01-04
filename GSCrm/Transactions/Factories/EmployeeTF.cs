using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeTF : TransactionFactory<EmployeeViewModel>
    {
        private readonly UserManager<User> userManager;
        public EmployeeTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        {
            userManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
        }

        protected override void CreateHandler(OperationType operationType, EmployeeViewModel employeeViewModel)
        {
            Organization currentOrganization = default;
            if (operationType.IsInList(baseOperationTypes.With(OperationType.ChangeEmployeeDivision, OperationType.UnlockEmployee)))
            {
                currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
            }
            if (operationType.IsInList(OperationType.ChangeEmployeeDivision, OperationType.UnlockEmployee))
            {
                List<Division> allDivisions = currentOrganization.GetDivisions(context);
                transaction.AddParameter("AllDivisions", allDivisions);
            }
            if (operationType == OperationType.ChangeEmployeeDivision)
            {
                Employee employee = context.Employees.AsNoTracking().Include(pos => pos.EmployeePositions).FirstOrDefault(i => i.Id == employeeViewModel.Id);
                transaction.AddParameter("Employee", employee);
            }
            if (operationType == OperationType.UnlockEmployee)
            {
                Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == employeeViewModel.Id);
                transaction.AddParameter("Employee", employee);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            transaction.AddParameter("CurrentOrganization", currentOrganization);
        }

        protected override void CloseHandler(TransactionStatus transactionStatus)
        {
            if (transactionStatus == TransactionStatus.Error)
            {
                User user = transaction.GetParameterValue("UserAccount") as User;
                if (user != null) userManager.DeleteAsync(user);
            }
        }
    }
}

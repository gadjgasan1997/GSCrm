using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.DataTransformers
{
    public class EmployeeTransformer : BaseTransformer<Employee, EmployeeViewModel>
    {
        private readonly User currentUser;
        public EmployeeTransformer(ApplicationDbContext context, ResManager resManager, HttpContext httpContext = null) : base (context, resManager)
        {
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        public override Employee OnModelCreate(EmployeeViewModel employeeViewModel)
        {
            Organization currentOrganization = employeeViewModel.GetOrganization(context);
            Division division = employeeViewModel.GetDivision(currentOrganization);
            Position primaryPosition = employeeViewModel.GetPrimaryPosition(division);

            Employee employee = new Employee()
            {
                FirstName = employeeViewModel.FirstName,
                LastName = employeeViewModel.LastName,
                MiddleName = employeeViewModel.MiddleName,
                PrimaryPositionId = primaryPosition.Id,
                DivisionId = division.Id,
                UserId = Guid.Parse(employeeViewModel.UserId)
            };

            EmployeePosition employeePosition = new EmployeePosition()
            {
                Employee = employee,
                EmployeeId = employee.Id,
                Position = primaryPosition,
                PositionId = primaryPosition.Id
            };
            context.Entry(employeePosition).State = EntityState.Added;
            employee.EmployeePositions.Add(employeePosition);
            return employee;
        }

        public override Employee OnModelUpdate(EmployeeViewModel employeeViewModel)
        {
            Employee oldEmployee = context.Employees
                .Include(empPos => empPos.EmployeePositions)
                .Include(empCon => empCon.EmployeeContacts)
                .FirstOrDefault(i => i.Id == employeeViewModel.Id);
            oldEmployee.FirstName = employeeViewModel.FirstName;
            oldEmployee.LastName = employeeViewModel.LastName;
            oldEmployee.MiddleName = employeeViewModel.MiddleName;
            return oldEmployee;
        }

        public override EmployeeViewModel DataToViewModel(Employee employee)
        {
            Position primaryPosition = employee.GetPrimaryPosition(context);
            Position parentPosition = primaryPosition == null ? null : primaryPosition.GetParentPosition(context);
            Employee supervisor = parentPosition == null ? null : context.Employees.FirstOrDefault(i => i.Id == parentPosition.PrimaryEmployeeId);
            Division division = employee.GetDivision(context);
            Organization organization = division.GetOrganization(context);
            return new EmployeeViewModel()
            {
                Id = employee.Id,
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
                DivisionId = division.Id,
                DivisionName = division.Name,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                FullName = employee.GetFullName(),
                FullInitialName = employee.GetIntialsFullName(),
                PrimaryPositionId = primaryPosition?.Id,
                PrimaryPositionName = primaryPosition?.Name,
                EmployeeStatus = employee.EmployeeStatus.ToString(),
                SupervisorId = supervisor?.Id.ToString(),
                SupervisorInitialName = supervisor?.GetIntialsFullName()
            };
        }

        public override EmployeeViewModel UpdateViewModelFromCash(EmployeeViewModel employeeViewModel)
        {
            EmployeeViewModel empPosViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_POSITIONS);
            EmployeeViewModel empContactViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_CONTACTS);
            EmployeeViewModel empSubViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_SUBS);
            employeeViewModel.SearchPosName = empPosViewModelCash.SearchPosNameCash.GetValueOrDefault(currentUser.Id);
            employeeViewModel.SearchParentPosName = empPosViewModelCash.SearchParentPosNameCash.GetValueOrDefault(currentUser.Id);
            employeeViewModel.SearchContactType = empContactViewModelCash.SearchContactTypeCash.GetValueOrDefault(currentUser.Id);
            employeeViewModel.SearchContactPhone = empContactViewModelCash.SearchContactPhoneCash.GetValueOrDefault(currentUser.Id);
            employeeViewModel.SearchContactEmail = empContactViewModelCash.SearchContactEmailCash.GetValueOrDefault(currentUser.Id);
            employeeViewModel.SearchSubordinateFullName = empSubViewModelCash.SearchSubordinateFullNameCash.GetValueOrDefault(currentUser.Id);
            return employeeViewModel;
        }
    }
}

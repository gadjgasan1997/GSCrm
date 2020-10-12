using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class OrganizationHelpers
    {
        /// <summary>
        /// Возвращает все подразделения организации
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Division> GetDivisions(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
        {
            Organization organization = context.Organizations
                .Include(div => div.Divisions)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == orgViewModel.Id);
            return organization.Divisions;
        }

        /// <summary>
        /// Возвращает все должности организации
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Position> GetPositions(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
        {
            List<Position> positions = new List<Position>();
            var test = orgViewModel.GetDivisions(context);
            test.ForEach(division => positions.AddRange(division.Positions));
            return positions;
        }

        /// <summary>
        /// Возвращает всех сотрудников организации
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Employee> GetEmployees(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
        {
            List<Employee> employees = new List<Employee>();
            List<Division> orgDivisions = orgViewModel.GetDivisions(context);
            orgDivisions.ForEach(division => employees.AddRange(division.GetEmployees(context)));
            return employees;
        }

        /// <summary>
        /// Возвращает всех полномочий организации
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Responsibility> GetResponsibilities(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Responsibilities.Where(resp => resp.OrganizationId == orgViewModel.Id).ToList();
    }
}

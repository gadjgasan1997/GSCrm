using GSCrm.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class PositionViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public PositionStatus PositionStatus { get; set; }
        public PositionLockReason PositionLockReason { get; set; }
        public bool IsPrimary { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public Guid? ParentPositionId { get; set; }
        public string ParentPositionName { get; set; }
        public Guid? PrimaryEmployeeId { get; set; }
        public string PrimaryEmployeeInitialName { get; set; }

        /// <summary>
        /// Поиск по сотрудникам, занимающим эту должность
        /// </summary>
        #region Position Employees Search
        /// <summary>
        /// Поиск по инициалам сотрудника
        /// </summary>
        public string SearchEmployeeInitialName { get; set; }
        #endregion

        /// <summary>
        /// Поиск по дочерним должностям
        /// </summary>
        #region Position Sub Positions Search
        /// <summary>
        /// Поиск по названию должности
        /// </summary>
        public string SearchSubPositionName { get; set; }

        /// <summary>
        /// Поиск по основному сотруднику на должности
        /// </summary>
        public string SearchSubPositionPrimaryEmployee { get; set; }
        #endregion

        #region Linked Lists
        /// <summary>
        /// Иерархия родительских должностей
        /// </summary>
        public List<PositionViewModel> PositionsHierarchy { get; set; } = new List<PositionViewModel>();
        /// <summary>
        /// Список сотрудников, занимающих эту должность
        /// </summary>
        public List<EmployeeViewModel> PositionEmployees { get; set; } = new List<EmployeeViewModel>();
        /// <summary>
        /// Список дочерних должностей
        /// </summary>
        public List<PositionViewModel> SubPositions { get; set; } = new List<PositionViewModel>();
        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        public string UserId { get; set; }
        /// <summary>
        /// Признак при создании сотрудника, создается ли новый аккаунт для него или выбран существующий
        /// </summary>
        public bool UserAccountExists { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string FullInitialName { get; set; }
        public string SupervisorId { get; set; }
        public string SupervisorInitialName { get; set; }
        public Guid DivisionId { get; set; }
        public string DivisionName { get; set; }
        public Guid? PrimaryPositionId { get; set; }
        public string PrimaryPositionName { get; set; }
        public string EmployeeStatus { get; set; } = "None";
        public string EmployeeLockReason { get; set; } = "None";

        /// <summary>
        /// Поиск по должностям сотрудника
        /// </summary>
        #region Employee Position Search
        /// <summary>
        /// Поиск по названию должности
        /// </summary>
        public string SearchPosName { get; set; }

        /// <summary>
        /// Поиск по названию родительской должности
        /// </summary>
        public string SearchParentPosName { get; set; }
        #endregion

        /// <summary>
        /// Поиск по контактам сотрудника
        /// </summary>
        #region Employee Contact Search
        /// <summary>
        /// Поиск по типу контакта
        /// </summary>
        public string SearchContactType { get; set; } = string.Empty;

        /// <summary>
        /// Поиск по номеру телефона
        /// </summary>
        public string SearchContactPhone { get; set; }

        /// <summary>
        /// Поиск по почте
        /// </summary>
        public string SearchContactEmail { get; set; }
        #endregion

        /// <summary>
        /// Поиск по подчиненным сотрудника
        /// </summary>
        #region Employee Contact Search
        public string SearchSubordinateFullName { get; set; }
        #endregion

        /// <summary>
        /// Поиск в модальном окне управления должностями сотрудника
        /// </summary>
        #region Employee Position Management Search
        /// <summary>
        /// Поиск по названию должности среди всех должностей организации
        /// </summary>
        public string SearchAllPosName { get; set; }

        /// <summary>
        /// Поиск по названию родительской должности среди всех должностей организации
        /// </summary>
        public string SearchAllParentPosName { get; set; }

        /// <summary>
        /// Поиск по названию должности среди выбранных должностей сотрудника
        /// </summary>
        public string SearchSelectedPosName { get; set; }

        /// <summary>
        /// Поиск по названию родительской должности среди выбранных должностей сотрудника
        /// </summary>
        public string SearchSelectedParentPosName { get; set; }
        #endregion

        /// <summary>
        /// Поиск в модельном окне управления полномочиями сотрудника
        /// </summary>
        #region Employee Position Management Search
        /// <summary>
        /// Поиск по названию полномочия среди всех полномочий организации
        /// </summary>
        public string SearchAllRespName { get; set; }

        /// <summary>
        /// Поиск по названию полномочия среди выбранных полномочий сотрудника
        /// </summary>
        public string SearchSelectedRespName { get; set; }
        #endregion

        #region Linked Lists
        /// <summary>
        /// Список с подчиненными сотрудника
        /// </summary>
        public List<EmployeeViewModel> SubordinatesViewModels { get; set; } = new List<EmployeeViewModel>();
        /// <summary>
        /// Список с должностями сотрудника
        /// </summary>
        public List<EmployeePositionViewModel> EmployeePositionViewModels { get; set; } = new List<EmployeePositionViewModel>();
        /// <summary>
        /// Список с контактами сотрудника
        /// </summary>
        public List<EmployeeContactViewModel> EmployeeContactViewModels { get; set; } = new List<EmployeeContactViewModel>();
        #endregion
    }
}

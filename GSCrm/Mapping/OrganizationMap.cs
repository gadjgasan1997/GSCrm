using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using System;
using System.Linq;
using static GSCrm.CommonConsts;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping
{
    public class OrganizationMap : BaseMap<Organization, OrganizationViewModel>
    {
        public OrganizationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        public override Organization OnModelCreate(OrganizationViewModel orgViewModel)
        {
            base.OnModelCreate(orgViewModel);
            orgViewModel.OwnerId = currentUser.Id;
            transaction.AddChange(currentUser, EntityState.Unchanged);

            // Добаление организации в список организаций пользователя
            Guid newOrgId = Guid.NewGuid();
            UserOrganization userOrganization = new UserOrganization()
            {
                Id = Guid.NewGuid(),
                OrganizationId = newOrgId,
                User = currentUser,
                UserId = currentUser.Id
            };
            transaction.AddChange(userOrganization, EntityState.Added);

            // Добавление настроек уведомлений у пользователя для этой организации
            OrgNotificationsSetting orgNotificationsSetting = new OrgNotificationsSetting()
            {
                Id = Guid.NewGuid(),
                UserOrganization = userOrganization,
                UserOrganizationId = userOrganization.Id
            };
            orgNotificationsSetting = new OrgNotificationsSettingMap(serviceProvider, context).InitNotSetting(orgNotificationsSetting);
            transaction.AddChange(orgNotificationsSetting, EntityState.Added);

            // Возврат результата
            return new Organization()
            {
                Id = newOrgId,
                Name = orgViewModel.Name,
                OwnerId = orgViewModel.OwnerId
            };
        }

        public override OrganizationViewModel DataToViewModel(Organization organization)
        {
            UserOrganization userOrganization = context.UserOrganizations.AsNoTracking()
                .FirstOrDefault(userOrg => userOrg.OrganizationId == organization.Id && userOrg.UserId == currentUser.Id);
            return new OrganizationViewModel()
            {
                Id = organization.Id,
                OwnerId = organization.OwnerId,
                Name = organization.Name,
                Accepted = userOrganization.Accepted
            };
        }

        /// <summary>
        /// Обновление модели из кеша
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="orgViewModel"></param>
        /// <param name="organizationViewTypes"></param>
        /// <returns></returns>
        public OrganizationViewModel Refresh(OrganizationViewModel orgViewModel, User currentUser, OrganizationViewType[] organizationViewTypes)
        {
            organizationViewTypes.ToList().ForEach(organizationViewType =>
            {
                switch (organizationViewType)
                {
                    // Восстановление данных поиска по подразделениям
                    case OrganizationViewType.DIVISIONS:
                        if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgDivView, DIVISIONS))
                        {
                            orgViewModel.SearchDivName = orgDivView.SearchDivName;
                            orgViewModel.SearchParentDivName = orgDivView.SearchParentDivName;
                        }
                        break;

                    // Восстановление данных поиска по должностям
                    case OrganizationViewType.POSITIONS:
                        if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgPosView, POSITIONS))
                        {
                            orgViewModel.SearchPosName = orgPosView.SearchPosName;
                            orgViewModel.SeacrhPositionDivName = orgPosView.SeacrhPositionDivName;
                            orgViewModel.SearchParentPosName = orgPosView.SearchParentPosName;
                            orgViewModel.SearchPrimaryEmployeeName = orgPosView.SearchPrimaryEmployeeName;
                        }
                        break;

                    // Восстановление данных поиска по сотрудникам
                    case OrganizationViewType.EMPLOYEES:
                        if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgEmpView, EMPLOYEES))
                        {
                            orgViewModel.SearchEmployeeName = orgEmpView.SearchEmployeeName;
                            orgViewModel.SeacrhEmployeeDivName = orgEmpView.SeacrhEmployeeDivName;
                            orgViewModel.SearchEmployeePrimaryPosName = orgEmpView.SearchEmployeePrimaryPosName;
                        }
                        break;

                    // Восстановление данных поиска по полномочиям
                    case OrganizationViewType.RESPONSIBILITIES:
                        if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel respEmpView, RESPONSIBILITIES))
                            orgViewModel.SeacrhResponsibilityName = respEmpView.SeacrhResponsibilityName;
                        break;

                    default:
                        break;
                }
            });
            return orgViewModel;
        }
    }
}

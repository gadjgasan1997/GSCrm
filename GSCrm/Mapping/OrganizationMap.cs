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
    }
}

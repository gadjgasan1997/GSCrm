using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.DataTransformers
{
    public class OrganizationTransformer : BaseTransformer<Organization, OrganizationViewModel>
    {
        private readonly User currentUser;
        public OrganizationTransformer(ApplicationDbContext context, ResManager resManager, HttpContext httpContext = null) : base(context, resManager)
        {
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        public override Organization OnModelCreate(OrganizationViewModel orgViewModel)
        {
            Guid newOrgId = Guid.NewGuid();
            User user = context.Users.FirstOrDefault(n => n.UserName == currentUser.UserName);
            orgViewModel.OwnerId = user.Id;
            return new Organization()
            {
                Id = newOrgId,
                Name = orgViewModel.Name,
                OwnerId = orgViewModel.OwnerId,
                UserOrganizations = new List<UserOrganization>()
                {
                    new UserOrganization()
                    {
                        OrganizationId = newOrgId,
                        User = user,
                        UserId = user.Id
                    }
                }
            };
        }

        public override OrganizationViewModel DataToViewModel(Organization organization)
        {
            return new OrganizationViewModel()
            {
                Id = organization.Id,
                OwnerId = organization.OwnerId,
                Name = organization.Name
            };
        }

        public override OrganizationViewModel UpdateViewModelFromCash(OrganizationViewModel orgViewModel)
        {
            OrganizationViewModel orgDivView = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, DIVISIONS);
            OrganizationViewModel orgPosView = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, POSITIONS);
            OrganizationViewModel orgEmpView = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, EMPLOYEES);
            orgViewModel.SearchDivName = orgDivView.SearchDivNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchParentDivName = orgDivView.SearchParentDivNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchPosName = orgPosView.SearchPosNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SeacrhPositionDivName = orgPosView.SeacrhPositionDivNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchParentPosName = orgPosView.SearchParentPosNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchPrimaryEmployeeName = orgPosView.SearchPrimaryEmployeeNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchEmployeeName = orgEmpView.SearchEmployeeNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SeacrhEmployeeDivName = orgEmpView.SeacrhEmployeeDivNameCash.GetValueOrDefault(currentUser.Id);
            orgViewModel.SearchEmployeePrimaryPosName = orgEmpView.SearchEmployeePrimaryPosNameCash.GetValueOrDefault(currentUser.Id);
            return orgViewModel;
        }
    }
}

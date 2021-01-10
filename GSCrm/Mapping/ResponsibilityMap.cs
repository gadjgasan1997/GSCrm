using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;
using GSCrm.Data;

namespace GSCrm.Mapping
{
    public class ResponsibilityMap : BaseMap<Responsibility, ResponsibilityViewModel>
    {
        public ResponsibilityMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override ResponsibilityViewModel DataToViewModel(Responsibility responsibility)
        {
            return new ResponsibilityViewModel()
            {
                Id = responsibility.Id,
                Name = responsibility.Name,
                OrganizationId = responsibility.OrganizationId,
                OrganizationName = context.Organizations.FirstOrDefault(i => i.Id == responsibility.OrganizationId)?.Name,
                OrgDelete = responsibility.OrgDelete,
                DivCreate = responsibility.DivCreate,
                DivDelete = responsibility.DivDelete,
                PosCreate = responsibility.PosCreate,
                PosDelete = responsibility.PosDelete,
                PosUpdate = responsibility.PosUpdate,
                PosChangeDiv = responsibility.PosChangeDiv,
                PosUnlock = responsibility.PosUnlock,
                EmpCreate = responsibility.EmpCreate,
                EmpDelete = responsibility.EmpDelete,
                EmpUpdate = responsibility.EmpUpdate,
                EmpChangeDiv = responsibility.EmpChangeDiv,
                EmpUnlock = responsibility.EmpUnlock,
                EmpPossManagement = responsibility.EmpPossManagement,
                EmpRespsManagement = responsibility.EmpRespsManagement,
                EmpContactCreate = responsibility.EmpContactCreate,
                EmpContactUpdate = responsibility.EmpContactUpdate,
                EmpContactDelete = responsibility.EmpContactDelete,
                RespCreate = responsibility.RespCreate,
                RespUpdate = responsibility.RespUpdate,
                RespDelete = responsibility.RespDelete,
                AccAddressCreate = responsibility.AccAddressCreate,
                AccAddressDelete = responsibility.AccAddressDelete,
                AccAddressUpdate = responsibility.AccAddressUpdate,
                AccChangeType = responsibility.AccChangeType,
                AccUnlock = responsibility.AccUnlock,
                AccContactCreate = responsibility.AccContactCreate,
                AccContactDelete = responsibility.AccContactDelete,
                AccContactUpdate = responsibility.AccContactUpdate,
                AccCreate = responsibility.AccCreate,
                AccDelete = responsibility.AccDelete,
                AccInvoiceCreate = responsibility.AccInvoiceCreate,
                AccInvoiceDelete = responsibility.AccInvoiceDelete,
                AccInvoiceUpdate = responsibility.AccInvoiceUpdate,
                AccTeamManagement = responsibility.AccTeamManagement,
                AccUpdate = responsibility.AccUpdate
            };
        }

        public override Responsibility OnModelCreate(ResponsibilityViewModel responsibilityViewModel) => GetResponsibility(responsibilityViewModel);

        public override Responsibility OnModelUpdate(ResponsibilityViewModel responsibilityViewModel)
        {
            Responsibility responsibility = GetResponsibility(responsibilityViewModel);
            responsibility.Id = responsibilityViewModel.Id;
            return responsibility;
        }

        /// <summary>
        /// Выполняет общие действие для методов "OnModelCreate" и "OnModelUpdate"
        /// </summary>
        /// <param name="responsibilityViewModel"></param>
        /// <returns></returns>
        private Responsibility GetResponsibility(ResponsibilityViewModel responsibilityViewModel)
        {
            return new Responsibility()
            {
                Name = responsibilityViewModel.Name,
                OrganizationId = responsibilityViewModel.OrganizationId,
                OrgDelete = responsibilityViewModel.OrgDelete,
                DivCreate = responsibilityViewModel.DivCreate,
                DivDelete = responsibilityViewModel.DivDelete,
                PosCreate = responsibilityViewModel.PosCreate,
                PosDelete = responsibilityViewModel.PosDelete,
                PosUpdate = responsibilityViewModel.PosUpdate,
                PosChangeDiv = responsibilityViewModel.PosChangeDiv,
                PosUnlock = responsibilityViewModel.PosUnlock,
                EmpCreate = responsibilityViewModel.EmpCreate,
                EmpDelete = responsibilityViewModel.EmpDelete,
                EmpUpdate = responsibilityViewModel.EmpUpdate,
                EmpChangeDiv = responsibilityViewModel.EmpChangeDiv,
                EmpUnlock = responsibilityViewModel.EmpUnlock,
                EmpPossManagement = responsibilityViewModel.EmpPossManagement,
                EmpRespsManagement = responsibilityViewModel.EmpRespsManagement,
                EmpContactCreate = responsibilityViewModel.EmpContactCreate,
                EmpContactUpdate = responsibilityViewModel.EmpContactUpdate,
                EmpContactDelete = responsibilityViewModel.EmpContactDelete,
                RespCreate = responsibilityViewModel.RespCreate,
                RespUpdate = responsibilityViewModel.RespUpdate,
                RespDelete = responsibilityViewModel.RespDelete,
                AccAddressCreate = responsibilityViewModel.AccAddressCreate,
                AccAddressDelete = responsibilityViewModel.AccAddressDelete,
                AccAddressUpdate = responsibilityViewModel.AccAddressUpdate,
                AccChangeType = responsibilityViewModel.AccChangeType,
                AccUnlock = responsibilityViewModel.AccUnlock,
                AccContactCreate = responsibilityViewModel.AccContactCreate,
                AccContactDelete = responsibilityViewModel.AccContactDelete,
                AccContactUpdate = responsibilityViewModel.AccContactUpdate,
                AccCreate = responsibilityViewModel.AccCreate,
                AccDelete = responsibilityViewModel.AccDelete,
                AccInvoiceCreate = responsibilityViewModel.AccInvoiceCreate,
                AccInvoiceDelete = responsibilityViewModel.AccInvoiceDelete,
                AccInvoiceUpdate = responsibilityViewModel.AccInvoiceUpdate,
                AccTeamManagement = responsibilityViewModel.AccTeamManagement,
                AccUpdate = responsibilityViewModel.AccUpdate
            };
        }
    }
}

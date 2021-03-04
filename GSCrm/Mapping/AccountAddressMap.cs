using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;

namespace GSCrm.Mapping
{
    public class AccountAddressMap : BaseMap<AccountAddress, AccountAddressViewModel>
    {
        public AccountAddressMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AccountAddressViewModel DataToViewModel(AccountAddress accountAddress)
        {
            return new AccountAddressViewModel()
            {
                Id = accountAddress.Id,
                AccountId = accountAddress.AccountId.ToString(),
                AddressType = accountAddress.AddressType.ToString(),
                Country = accountAddress.Country,
                Region = accountAddress.Region,
                City = accountAddress.City,
                Street = accountAddress.Street,
                House = accountAddress.House,
                FullAddress = accountAddress.GetFullAddress(currentUser)
            };
        }

        public override AccountAddress OnModelCreate(AccountAddressViewModel addressViewModel)
        {
            base.OnModelCreate(addressViewModel);
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            AddressType addressType = (AddressType)transaction.GetParameterValue("AddressType");
            return new AccountAddress()
            {
                AccountId = account.Id,
                AddressType = addressType,
                Country = addressViewModel.Country,
                Region = addressViewModel.Region,
                City = addressViewModel.City,
                Street = addressViewModel.Street,
                House = addressViewModel.House
            };
        }

        public override AccountAddress OnModelUpdate(AccountAddressViewModel addressViewModel)
        {
            AccountAddress oldAccountAddress = base.OnModelUpdate(addressViewModel);
            oldAccountAddress.AddressType = (AddressType)transaction.GetParameterValue("NewAddressType");
            oldAccountAddress.Country = addressViewModel.Country;
            oldAccountAddress.Region = addressViewModel.Region;
            oldAccountAddress.City = addressViewModel.City;
            oldAccountAddress.Street = addressViewModel.Street;
            oldAccountAddress.House = addressViewModel.House;
            return oldAccountAddress;
        }
    }
}

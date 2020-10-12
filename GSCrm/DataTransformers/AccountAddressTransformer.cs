using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class AccountAddressTransformer : BaseTransformer<AccountAddress, AccountAddressViewModel>
    {
        private readonly User currentUser;
        public AccountAddressTransformer(ApplicationDbContext context, ResManager resManager, User currentUser = null) : base(context, resManager)
        {
            this.currentUser = currentUser;
        }

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
            return new AccountAddress()
            {
                AccountId = Guid.Parse(addressViewModel.AccountId),
                AddressType = (AddressType)Enum.Parse(typeof(AddressType), addressViewModel.AddressType),
                Country = addressViewModel.Country,
                Region = addressViewModel.Region,
                City = addressViewModel.City,
                Street = addressViewModel.Street,
                House = addressViewModel.House
            };
        }

        public override AccountAddress OnModelUpdate(AccountAddressViewModel addressViewModel)
        {
            AccountAddress oldAccountAddress = context.AccountAddresses.FirstOrDefault(i => i.Id == addressViewModel.Id);
            oldAccountAddress.AddressType = (AddressType)Enum.Parse(typeof(AddressType), addressViewModel.AddressType);
            oldAccountAddress.Country = addressViewModel.Country;
            oldAccountAddress.Region = addressViewModel.Region;
            oldAccountAddress.City = addressViewModel.City;
            oldAccountAddress.Street = addressViewModel.Street;
            oldAccountAddress.House = addressViewModel.House;
            return oldAccountAddress;
        }
    }
}

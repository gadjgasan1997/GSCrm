using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.AppUtils;

namespace GSCrm.Helpers
{
    public static class AccountAddressHelpers
    {
        public static string GetFullAddress(this AccountAddress address, User currentUser = null)
            => new StringBuilder()
                .Append(address.Country).Append(", ")
                .Append(GetLocationPrefix(REGION_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.Region).Append(", ")
                .Append(GetLocationPrefix(CITY_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.City).Append(", ")
                .Append(GetLocationPrefix(STREET_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.Street).Append(", ")
                .Append(GetLocationPrefix(HOUSE_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.House).ToString();

        public static string ToLocalString(this AddressType addressType)
            => addressType switch
            {
                AddressType.None => "Не указан",
                AddressType.Legal => "Юридический",
                AddressType.Other => "Прочий",
                AddressType.Postal => "Почтовый",
                _ => string.Empty
            };

        public static Account GetAccount(this AccountAddress accountAddress, ApplicationDbContext context)
            => context.Accounts.AsNoTracking().FirstOrDefault(acc => acc.Id == accountAddress.AccountId);
    }
}

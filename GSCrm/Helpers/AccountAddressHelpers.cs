using GSCrm.Localization;
using GSCrm.Models;
using System.Text;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.AppUtils;

namespace GSCrm.Helpers
{
    public static class AccountAddressHelpers
    {
        public static string GetFullAddress(this AccountAddress address, User currentUser)
        {
            return new StringBuilder()
                .Append(address.Country).Append(", ")
                .Append(GetLocationPrefix(REGION_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.Region).Append(", ")
                .Append(GetLocationPrefix(CITY_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.City).Append(", ")
                .Append(GetLocationPrefix(STREET_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.Street).Append(", ")
                .Append(GetLocationPrefix(HOUSE_KEY, currentUser?.DefaultLanguage)).Append(" ")
                .Append(address.House)
                .ToString();
        }
    }
}

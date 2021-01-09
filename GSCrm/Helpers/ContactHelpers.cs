using GSCrm.Models;
using GSCrm.Models.Enums;

namespace GSCrm.Helpers
{
    public static class ContactHelpers
    {
        public static string GetFullName(this AccountContact accountContact)
            => $"{accountContact.LastName} {accountContact.FirstName} {accountContact.MiddleName}";

        public static string ToLocalString(this ContactType contactType)
            => contactType switch
            {
                ContactType.None => "Не указан",
                ContactType.Personal => "Личный",
                ContactType.Work => "Рабочий",
                _ => string.Empty
            };
    }
}

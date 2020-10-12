using GSCrm.Models;

namespace GSCrm.Helpers
{
    public static class AccountContactHelpers
    {
        public static string GetFullName(this AccountContact accountContact)
            => $"{accountContact.LastName} {accountContact.FirstName} {accountContact.MiddleName}";
    }
}

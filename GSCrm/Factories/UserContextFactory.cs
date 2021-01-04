using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GSCrm.Factories
{
    public class UserContextFactory : IUserContextFactory
    {
        private readonly IHttpContextAccessor contextAccessor;
        private ClaimsPrincipal _userContext;
        public UserContextFactory(IHttpContextAccessor accessor)
        {
            contextAccessor = accessor;
        }

        private HttpContext Context => contextAccessor.HttpContext;

        HttpContext IUserContextFactory.HttpContext => contextAccessor.HttpContext;

        public ClaimsPrincipal GetUserContext()
        {
            if (_userContext == null && IsUserAuthenticated() == true)
                _userContext = new ClaimsPrincipal() { };
            return _userContext;
        }

        public bool? IsUserAuthenticated() => Context?.User?.Identity?.IsAuthenticated;
    }
}

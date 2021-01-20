using Microsoft.AspNetCore.Http;

namespace GSCrm.Factories
{
    public class UserContextFactory : IUserContextFactory
    {
        private readonly IHttpContextAccessor contextAccessor;
        public UserContextFactory(IHttpContextAccessor accessor)
        {
            contextAccessor = accessor;
        }

        HttpContext IUserContextFactory.HttpContext => contextAccessor.HttpContext;
    }
}

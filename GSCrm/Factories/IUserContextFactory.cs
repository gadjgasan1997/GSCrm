using Microsoft.AspNetCore.Http;

namespace GSCrm.Factories
{
    public interface IUserContextFactory
    {
        HttpContext HttpContext { get; }
    }
}

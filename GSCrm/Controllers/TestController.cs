using GSCrm.Data.ApplicationInfo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GSCrm.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        private readonly IViewsInfo viewsInfo;
        public TestController(IViewsInfo viewsInfo)
        {
            this.viewsInfo = viewsInfo;
        }
    }
}

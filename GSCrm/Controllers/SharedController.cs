using Microsoft.AspNetCore.Mvc;

namespace GSCrm.Controllers
{
    public class SharedController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();

        public IActionResult ViewNotFound() => View();
    }
}

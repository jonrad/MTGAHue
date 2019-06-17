using Microsoft.AspNetCore.Mvc;

namespace MagicLights.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

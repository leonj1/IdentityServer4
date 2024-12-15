using Microsoft.AspNetCore.Mvc;

namespace MvcImplicit.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

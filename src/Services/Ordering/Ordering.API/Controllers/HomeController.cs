using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}

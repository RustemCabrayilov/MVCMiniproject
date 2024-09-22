using Microsoft.AspNetCore.Mvc;

namespace OMMS.UI.Areas.Admin.Controllers
{
    [Area("admin")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

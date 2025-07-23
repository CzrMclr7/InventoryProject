using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

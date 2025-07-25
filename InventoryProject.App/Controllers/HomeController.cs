using InventoryProject.App.Models;
using InventoryProject.DataAccess.DataContextModels;
using Microsoft.AspNetCore.Authorization;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserModuleAccessRepository _userModuleAccessRepo;

        protected int UserId => int.Parse(User.Identity.Name);

        public HomeController(ILogger<HomeController> logger, IUserModuleAccessRepository userModuleAccessRepo)
        {
            _logger = logger;
            _userModuleAccessRepo = userModuleAccessRepo;
        }

        public IActionResult Index()
        {
            //var accessList = _userModuleAccessRepo.GetUserModuleAccess(UserId);

            //ViewBag.UserAccess = accessList;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

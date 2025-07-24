using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    public class UserModuleAccessController : Controller
    {
        private readonly IUserModuleAccessRepository _userModuleAccessRepo;

        protected int UserId => int.Parse(User.Identity.Name);

        public UserModuleAccessController(IUserModuleAccessRepository userModuleAccessRepo)
        {
            _userModuleAccessRepo = userModuleAccessRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetUserModuleAccess(int id)
        {
            var user = await _userModuleAccessRepo.GetUserModuleAccess(id);
            return Ok(user);
        }
    }
}

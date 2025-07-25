using InventoryProject.App.ViewModels;
using InventoryProject.DataAccess.Models;
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
        public async Task<IActionResult> GetUserModuleAccess()
        {
            var user = await _userModuleAccessRepo.GetUserModuleAccess(UserId);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBulk([FromBody] List<UserModuleAccessModel> userModuleAccess)
        {
            try
            {
                var data = await _userModuleAccessRepo.SaveBulkAsync(userModuleAccess, UserId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

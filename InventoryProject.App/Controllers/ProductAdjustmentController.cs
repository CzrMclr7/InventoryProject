using InventoryProject.App.ViewModels;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class ProductAdjustmentController : Controller
    {
        private readonly IProductAdjustmentRepository _repo;
        private readonly IUserModuleAccessRepository _userModuleAccessRepo;
        protected int UserId => int.Parse(User.Identity.Name);
        public ProductAdjustmentController(IProductAdjustmentRepository repo, IUserModuleAccessRepository userModuleAccessRepo)
        {
            _repo = repo;
            _userModuleAccessRepo = userModuleAccessRepo;
        }
        public async Task<IActionResult> Index()
        {
            var accessRights = await _userModuleAccessRepo.GetUserModuleAccess(UserId);

            var salesAccess = accessRights.FirstOrDefault(x => x.ModuleCode == "SALES");

            if (salesAccess == null || !salesAccess.CanView)
                return Forbid();

            var viewModel = new ProductAdjustmentViewModel
            {
                ProductAdjustments = (await _repo.Inq(null)).ToList(),
                AccessRights = accessRights.FirstOrDefault(x => x.ModuleCode == "SALES")
            };

            return View(viewModel);


            // in html
            //@if(Model.AccessRights.CanAdd)
            //{
            //    < button class="btn btn-primary">Add Sales</button>
            //}

            //@if(Model.AccessRights.CanEdit)
            //        {
            //    < button class="btn btn-warning">Edit</button>
            //}

            //@if(Model.AccessRights.CanDelete)
            //{
            //    < button class= "btn btn-danger" > Delete </ button >
            //}
        }

        [HttpGet]
        public async Task<IActionResult> Inq(int? id)
        {
            var product = await _repo.Inq(id);

            //var model = new ProductAdjustmentViewModel
            //{
            //    ProductAdjustments = (List<ProductAdjustmentModel>)product
            //};


            return Ok(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveIn(ProductAdjustmentModel model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    return BadRequest(ModelState);

                var data = await _repo.SaveAsync(model, UserId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOut(ProductAdjustmentModel model)
        {
            try
            {
                var data = await _repo.SaveAsync(model, UserId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

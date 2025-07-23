using InventoryProject.App.ViewModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class ProductAdjustmentController : Controller
    {
        private readonly IProductAdjustmentRepository _repo;
        public ProductAdjustmentController(IProductAdjustmentRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
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
                int userId = int.Parse(User.Identity.Name ?? "0");

                var data = await _repo.SaveAsync(model, userId);

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
                int userId = int.Parse(User.Identity.Name ?? "0");

                var data = await _repo.SaveAsync(model, userId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

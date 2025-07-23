using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repo;

        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var model = new ProductModel();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> Inq(int? id)
        {
            var product = await _repo.Inq(id);
            return Ok(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ProductModel model)
        {
            try
            {
                int userId = int.Parse(User.Identity.Name ?? "0");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var data = await _repo.SaveAsync(model, userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("No IDs provided.");
                var idList = id
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(i => int.TryParse(i, out var parsedId) ? parsedId : (int?)null)
                                .Where(i => i.HasValue)
                                .Select(i => i.Value)
                                .ToList();
                if (!idList.Any())
                    return BadRequest("Invalid ID list.");
                await _repo.BatchDeleteAsync(idList);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
using InventoryProject.App.ViewModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ISalesRepository _salesRepo;
        private readonly ISalesDetailRepository _salesDetailRepo;

        public SalesController(
            ISalesRepository salesRepo,
            ISalesDetailRepository salesDetailRepo)
        {
            _salesRepo = salesRepo;
            _salesDetailRepo = salesDetailRepo;
        }

        #region Views

        public IActionResult Index()
        {
            var vModel = new SalesViewModel
            {
                Sales = new SalesModel(),
                SalesDetails = new List<SalesDetailModel>()
            };

            return View(vModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var salesDetails = await _salesDetailRepo.GetByIdAsync(id);
            var viewModel = new SalesViewModel
            {
                SalesDetails = new List<SalesDetailModel> { salesDetails }
            };

            return View(viewModel);
        }

        #endregion Views

        #region APIs Getters

        [HttpGet]
        public async Task<IActionResult> GetAll(int id)
        {
            try
            {
                var salesDetails = await _salesDetailRepo.Inq(id);
                return Ok(salesDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var sales = await _salesRepo.GetByIdAsync(id);
                
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Inq(int? id)
        {
            var sales = await _salesRepo.Inq(id);
            return Ok(sales);
        }

        #endregion APIs Getters

        #region APIs Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SalesViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                int userId = int.Parse(User.Identity.Name ?? "0");

                var data = await _salesRepo.SaveAsync(model.Sales, model.SalesDetails, userId); 

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
                await _salesRepo.BatchDeleteAsync(idList);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion APIs Actions
    }
}
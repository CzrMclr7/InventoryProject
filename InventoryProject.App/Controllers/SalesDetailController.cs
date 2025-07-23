using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class SalesDetailController : Controller
    {
        private readonly ISalesDetailRepository _salesDetailRepo;
        public SalesDetailController(ISalesDetailRepository salesDetailRepo)
        {
            _salesDetailRepo = salesDetailRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Inq(int? id)
        {
            var sales = await _salesDetailRepo.Inq(id);
            return Ok(sales);
        }


    }
}

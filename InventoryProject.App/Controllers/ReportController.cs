using InventoryProject.App.Models;
using InventoryProject.App.ViewModels.Report;
using InventoryProject.DataAccess.DataAccess.Interface;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models.Report;
using InventoryProject.DataAccess.Models.ReportFilters;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserRepo;
using InventoryProject.DataAccess.PredefinedReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IReportDA _reportDa;
        private readonly ISalesRepository _salesRepo;
        private readonly ISalesDetailRepository _salesDetailRepo;
        private readonly IProductAdjustmentRepository _productAdjustmentRepo;
        private readonly IUserRepository _userRepo;

        protected int UserId => int.Parse(User.Identity.Name);

        public ReportController(IProductRepository productRepo, IReportDA reportDa,
                        ISalesRepository salesRepo, ISalesDetailRepository salesDetailRepo, 
                        IProductAdjustmentRepository productAdjustmentRepo, IUserRepository userRepo)
        {
            _productRepo = productRepo;
            _reportDa = reportDa;
            _salesRepo = salesRepo;
            _salesDetailRepo = salesDetailRepo;
            _productAdjustmentRepo = productAdjustmentRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ProductsSummary()
        {
            try
            {
                //var companyId = int.Parse(User.FindFirstValue("Company"));
                ////var userAccess = await _userRepo.GetUserAccessByModule(userId, companyId, ModuleCodes.CONST_RPT_AP_AGING_DETAILS);
                //if (userAccess == null || !userAccess.CanRead)
                //    return View("AccessDenied");

                //var companyInfo = await _fileSetupDA.GetCompany(companyId);

                var reportFilterModel = new ReportFilterModel()
                {
                    ReportName = "Products Summary",
                    //CompanyId = companyId
                };

                var viewModel = new ReportViewModel()
                {
                    ReportFilter = reportFilterModel,
                    //Company = companyInfo
                };

                return View(viewModel);
            }
            catch (Exception ex) { return View("Error", new ErrorViewModel { Message = ex.Message, Exception = ex }); }
        }

        public async Task<IActionResult> ProductReport()
        {
            // Get product data from repository 
            var productList = await _productRepo.Inq(null);
            // Prepare data for the report 
            var reportData = new List<ProductReportModel>
            {
                new ProductReportModel
                {
                    Company = "MNLeistung Inc.",
                    CreatedBy = _userRepo.GetFirstNameById(UserId),
                    Title = "Product List Report",
                    Products = [..productList]
                }
            };
            // Create the report instance 
            var report = new ProductListReport
            {
                DataSource = reportData
            };

            // Export report to PDF stream 
            using var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            // Return the PDF as a downloadable file 
            return File(stream.ToArray(), "application/pdf", "ProductList.pdf");
        }

        public async Task<IActionResult> DetailedSalesReport()
        {

            // Get sales data from repository
            var salesList = await _salesRepo.GetDetailedSalesData(null);

            var reportData = new List<DetailedSalesReportModel>
            {
                new DetailedSalesReportModel
                {
                    Company = "MNLeistung Inc.",
                    CreatedBy = _userRepo.GetFirstNameById(UserId),
                    Title = "Product List Report",
                    Sales = [.. salesList]
                }
            };

            // Create the report instance 
            var report = new DetailedSalesReport()
            {
                DataSource = reportData
            };

           
            // Export report to PDF stream 
            using var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            // Return the PDF as a downloadable file 
            return File(stream.ToArray(), "application/pdf", "ProductList.pdf");
        }

        public async Task<IActionResult> ProductsSummaryReportData(ReportFilterModel reportFilter)
        {
            try
            {
                var companyId = int.Parse(User.FindFirstValue("Company"));
                //var userAccess = await _userRepo.GetUserAccessByModule(userId, companyId, ModuleCodes.CONST_RPT_AP_AGING_DETAILS);
                //if (userAccess == null || !userAccess.CanRead)
                //    return View("AccessDenied");

                //reportFilter.CompanyId = companyId;
                var data = await _reportDa.GetProductSummaryReportData(reportFilter);

                return Ok(data);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        public async Task<IActionResult> ProductAdjustmentReportAsync(string actionType = "All")
        {
            var productAdjusmentList = await _productAdjustmentRepo.GetProductAdjustmentData(actionType);

            var reportData = new List<ProductAdjustmentReportModel>
            {
                new ProductAdjustmentReportModel
                {
                    Company = "MNLeistung Inc.",
                    CreatedBy = _userRepo.GetFirstNameById(UserId),
                    Title = "Product Adjustment Report",
                    ProductAdjustments = [.. productAdjusmentList]
                }
            };

            var report = new ProductAdjustmentReport()
            {
                DataSource = reportData
            };

            // Export report to PDF stream 
            using var stream = new MemoryStream();
            report.ExportToPdf(stream);
            stream.Position = 0;

            // Return the PDF as a downloadable file 
            return File(stream.ToArray(), "application/pdf", "ProductAdjustment.pdf");
        }
    }
}

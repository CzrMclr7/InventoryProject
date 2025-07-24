using InventoryProject.App.Models;
using InventoryProject.App.ViewModels.Report;
using InventoryProject.DataAccess.DataAccess.Interface;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models.Report;
using InventoryProject.DataAccess.Models.ReportFilters;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesRepo;
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

        public ReportController(IProductRepository productRepo, IReportDA reportDa,
                        ISalesRepository salesRepo, ISalesDetailRepository salesDetailRepo)
        {
            _productRepo = productRepo;
            _reportDa = reportDa;
            _salesRepo = salesRepo;
            _salesDetailRepo = salesDetailRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductsSummary()
        {
            try
            {
                var userId = int.Parse(User.Identity.Name);
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
                    CreatedBy = "System",
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
            //// Get sales detail data from repository 
            //var salesDetailList = await _salesDetailRepo.Inq(null);
            // Prepare data for the report 
            //var reportData = await _salesRepo.GetDetailedSalesData(null);

            var reportData = new List<DetailedSalesReportModel>
            {
                new DetailedSalesReportModel
                {
                    Company = "MNLeistung Inc.",
                    CreatedBy = "System",
                    Title = "Product List Report",
                    Sales = [.. salesList]
                }
            };

            //reportData.Parameters["Company"].Value = "MNLeistung Inc.";
            //reportData.Parameters["CreatedBy"].Value = "System";
            //reportData.Parameters["Title"].Value = "Detailed Sales Report";
            //var reportData = new List<DetailedSalesReportModel>
            //{
            //    new DetailedSalesReportModel
            //    {
            //        Company = "MNLeistung Inc.",
            //        CreatedBy = "System",
            //        Title = "Detailed Sales Report",
            //        Sales = [.. salesList],
            //        SalesDetail = [.. salesDetailList]
            //    }
            //};
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
                var userId = int.Parse(User.Identity.Name);
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

        //public IActionResult ProductAdjustmentReport(string actionType = "All")
        //{
        //    // Example logic
        //    IEnumerable<ProductAdjustment> data;

        //    if (actionType == "IN")
        //        data = _context.ProductAdjustments.Where(p => p.Action == "IN").ToList();
        //    else if (actionType == "OUT")
        //        data = _context.ProductAdjustments.Where(p => p.Action == "OUT").ToList();
        //    else
        //        data = _context.ProductAdjustments.ToList();

        //    // Generate PDF using the filtered data...
        //    // Return PDF file

        //    return View(data); // or return File(...) if PDF is generated directly
        //}
    }
}

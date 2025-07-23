using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Models.ReportFilters;

namespace InventoryProject.App.ViewModels.Report
{
    public class ReportViewModel
    {
        public ReportFilterModel? ReportFilter { get; set; }
        //public CompanyModel? Company { get; set; }
        public UserModel? PrintedBy { get; set; }
    }
}

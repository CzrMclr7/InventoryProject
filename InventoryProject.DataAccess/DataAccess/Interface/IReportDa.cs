using InventoryProject.DataAccess.Models.Report;
using InventoryProject.DataAccess.Models.ReportFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.DataAccess.Interface
{
    public interface IReportDA
    {
        Task<IEnumerable<ProductReportModel>> GetProductSummaryReportData(ReportFilterModel reportFilter);
    }
}

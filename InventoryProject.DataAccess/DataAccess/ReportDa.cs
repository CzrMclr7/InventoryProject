using InventoryProject.DataAccess.DataAccess.Interface;
using InventoryProject.DataAccess.Models.Report;
using InventoryProject.DataAccess.Models.ReportFilters;
using InventoryProject.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.DataAccess
{
    public class ReportDa : IReportDA
    {
        private readonly ISqlDataAccessService _db;

        public ReportDa(ISqlDataAccessService db)
        {
            _db = db;
        }
        public async Task<IEnumerable<ProductReportModel>> GetProductSummaryReportData(ReportFilterModel reportFilter) =>
            await _db.LoadData<ProductReportModel, dynamic>("spProducts_Inq", new
            {
                //reportFilter.DateFrom,
                //reportFilter.DateTo,
                //reportFilter.AsOfDate,
                //reportFilter.DateRangeType,
                //reportFilter.Vendors,
                //reportFilter.Accounts,
                //reportFilter.CompanyId,
                //reportFilter.DateType
            });
    }
}

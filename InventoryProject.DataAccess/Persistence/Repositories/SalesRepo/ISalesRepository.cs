using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Models.Report;
using InventoryProject.DataAccess.PredefinedReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.SalesRepo
{
    public interface ISalesRepository
    {
        Task<SalesModel> GetByIdAsync(int id);
        Task BatchDeleteAsync(List<int> ids);
        Task<Sale> CreateAsync(Sale sales, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<SalesModel>> Inq(int? id);
        Task<Sale> UpdateAsync(Sale sales, int userId);
        Task<Sale> SaveAsync(SalesModel model, List<SalesDetailModel> details, int userId);
        Task<IEnumerable<SalesModel>> GetDetailedSalesData(int? id);
    }
}

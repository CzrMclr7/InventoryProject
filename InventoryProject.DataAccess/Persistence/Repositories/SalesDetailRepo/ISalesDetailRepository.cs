using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;

namespace InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo
{
    public interface ISalesDetailRepository
    {
        Task<SalesDetailModel> GetByIdAsync(int id);
        Task BatchDeleteAsync(List<int> ids);
        Task<SalesDetail> CreateAsync(SalesDetail salesDetail, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<SalesDetailModel>> Inq(int? id);
        Task<SalesDetail> SaveAsync(SalesDetailModel model, int userId);
        Task<SalesDetail> SaveNoRollBackAsync(SalesDetailModel model, int userId);
        Task<SalesDetail> UpdateAsync(SalesDetail salesDetail, int userId);
        Task BatchDeleteNoRollbackAsync(int[] ids);
        int GetOldSalesDetailQty(int salesId, int productId);
        //void UpdateProductTotalQty(int productId, int userId);

    }
}

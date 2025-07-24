using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;

namespace InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo
{
    public interface IProductAdjustmentRepository
    {
        Task<ProductAdjustmentModel> GetByIdAsync(int id);
        Task BatchDeleteAsync(List<int> ids);
        Task<ProductAdjustment> CreateAsync(ProductAdjustment productAdj, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductAdjustmentModel>> Inq(int? id);
        Task<ProductAdjustment> SaveAsync(ProductAdjustmentModel model, int userId);
        Task<ProductAdjustment> SaveNoRollBackAsync(ProductAdjustmentModel model, int userId);
        Task<ProductAdjustment> UpdateAsync(ProductAdjustment productAdj, int userId);
        Task BatchDeleteNoRollbackAsync(int[] ids);
        Task BatchDeleteNoRollbackAsyncBySalesDetailId(int[] ids);
        //string GetProductNameById(int id);
        //Task<Product> UpdateQtyById(SalesDetailModel salesDetailModel, int userId);
        Task<IEnumerable<ProductAdjustmentModel>> GetProductAdjustmentData(string? actionType);
    }
}

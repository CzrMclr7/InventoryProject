using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;

namespace InventoryProject.DataAccess.Persistence.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        Task<ProductModel> GetByIdAsync(int id);
        Task BatchDeleteAsync(List<int> ids);
        Task<Product> CreateAsync(Product product, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductModel>> Inq(int? id);
        Task<Product> SaveAsync(ProductModel model, int userId);
        Task<Product> SaveNoRollBackAsync(ProductModel model, int userId);
        Task<Product> UpdateAsync(Product product, int userId);
        string GetProductNameById(int id);
        Task<Product> UpdateProductQty(int productId, int qtyChange, string action, int? oldQty, int userId);
    }
}

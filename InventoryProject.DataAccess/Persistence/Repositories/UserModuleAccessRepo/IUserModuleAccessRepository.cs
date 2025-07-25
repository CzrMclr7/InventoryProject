using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo
{
    public interface IUserModuleAccessRepository
    {
        Task BatchDeleteAsync(List<int> ids);
        Task<UserModuleAccess> CreateAsync(UserModuleAccess userModuleAccess, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<UserModuleAccessModel>> Inq(int? id);
        Task<UserModuleAccess> SaveAsync(UserModuleAccessModel model, int userId);
        Task<UserModuleAccess> SaveNoRollBackAsync(UserModuleAccessModel model, int userId);
        Task<UserModuleAccess> UpdateAsync(UserModuleAccess userModuleAccess, int userId);
        Task BatchDeleteNoRollbackAsync(int[] ids);
        Task SaveBulkNoRollBackAsync(List<UserModuleAccessModel> model, int userId);
        Task<List<UserModuleAccess>> SaveBulkAsync(List<UserModuleAccessModel> modelList, int userId);
        Task<List<UserModuleAccessModel>> GetUserModuleAccess(int userId);
    }
}

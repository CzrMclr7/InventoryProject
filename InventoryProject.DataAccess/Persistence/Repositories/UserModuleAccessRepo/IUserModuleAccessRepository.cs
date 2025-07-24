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
        Task<List<UserModuleAccessModel>> GetUserModuleAccess(int userId);
    }
}

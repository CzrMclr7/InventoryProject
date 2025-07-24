using InventoryProject.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.ModuleRepo
{
    public interface IModuleRepository
    {
        Task<List<ModuleModel>> GetAllModules();
    }
}

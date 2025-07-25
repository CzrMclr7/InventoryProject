using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.ModuleRepo
{
    public class ModuleRepository: IModuleRepository
    {
        private readonly InventoryProjectDatabaseContext _context;

        public ModuleRepository(InventoryProjectDatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<ModuleModel>> GetAllModules()
        {
            var data = await _context.Modules
                .Select(m => new ModuleModel
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    ModuleCode = m.ModuleCode
                })
                .ToListAsync();
            return data;
        }
    }
}

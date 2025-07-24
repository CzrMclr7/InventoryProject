using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Services;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo
{
    public class UserModuleAccessRepository: IUserModuleAccessRepository
    {
        private readonly ISqlDataAccessService _db;
        public UserModuleAccessRepository(ISqlDataAccessService db)
        {
            _db = db;
        }

        public async Task<List<UserModuleAccessModel>> GetUserModuleAccess(int userId)
        {
            var data = await _db.LoadData<UserModuleAccessModel, dynamic>(
                "spUserModuleAccess_GetByUserId",
                new { userId },
                CommandType.StoredProcedure);
            return data.ToList();
        }
    }
}

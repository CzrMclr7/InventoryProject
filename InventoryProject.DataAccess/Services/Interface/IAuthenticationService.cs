using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Services.Interface
{
    public interface IAuthenticationService
    {
        Task<User> Authenticate(AuthRequest authRequest);
    }
}

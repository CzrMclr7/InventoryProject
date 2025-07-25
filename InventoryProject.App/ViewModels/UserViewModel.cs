using InventoryProject.DataAccess.Models;

namespace InventoryProject.App.ViewModels
{
    public class UserViewModel
    {
        public UserModel? User { get; set; }
        public List<UserModuleAccessModel>? UserModuleAccess { get; set; }
    }
}

using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;

namespace InventoryProject.DataAccess.Persistence.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User product, int userId);

        Task<User> SaveAsync(UserModel model, int userId);

        Task<User> SaveNoRollBackAsync(UserModel model, int userId);

        Task<User> UpdateAsync(User product, int userId);

        Task<User?> GetByUserNameAsync(string userName);

        Task<User?> GetUserByIdAsync(int id);
    }
}
using AutoMapper;
using DevExpress.CodeParser;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Models.Authentication;
using InventoryProject.DataAccess.Persistence.Repositories.ModuleRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using InventoryProject.DataAccess.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InventoryProject.DataAccess.Persistence.Repositories.UserRepo;

public class UserRepository : IUserRepository
{
    private readonly InventoryProjectDatabaseContext _context;
    private readonly EfCoreHelper<User> _contextHelper;
    private readonly ISqlDataAccessService _db;
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUserModuleAccessRepository _userModuleAccessRepository;

    public UserRepository(
        InventoryProjectDatabaseContext context,
        ISqlDataAccessService db,
        IMapper mapper,
        IModuleRepository moduleRepository,
        IUserModuleAccessRepository userModuleAccessRepository)
    {
        _context = context;
        _contextHelper = new EfCoreHelper<User>(context);
        _db = db;
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _userModuleAccessRepository = userModuleAccessRepository;
    }

    public async Task<User> SaveAsync(UserModel model, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var _model = _mapper.Map<User>(model);

            _model.PasswordSalt = "qWeRtY07";
            byte[] salt = Encoding.ASCII.GetBytes(_model.PasswordSalt);          

            if (model.Id == 0 && model.IsUpdate == false)
            {
                await ValidateUserAsync(_model);
                await ValidateConfirmPassword(model);
                _model.Password = GenerateHashedPassword(_model.Password, salt);
                _model = await CreateAsync(_model, userId);

                var modules = await _moduleRepository.GetAllModules();
                // Add code to include adding UserModuleAccess
                var accessList = modules.Select(m => new UserModuleAccessModel
                {
                    UserId = _model.Id,
                    ModuleId = m.Id,
                    CanView = true,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false
                }).ToList();

                if (userId == 0)
                    userId = _model.Id;

                await _userModuleAccessRepository.SaveBulkNoRollBackAsync(accessList, userId);
            }
            else
            {
                User user;

                if (model.NewPassword != null && model.NewPassword != "")
                {
                    await ValidateConfirmPassword(model);
                    user = await GetByUserNameAsync(model.Username);
                    user.Password = GenerateHashedPassword(model.NewPassword, salt);
                }
                else
                {
                    user = await GetUserByIdAsync(model.Id);
                    user.Age = model.Age;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IsAdmin = model.IsAdmin;
                    user.ProfilePicture = model.ProfilePicture;
                }                

                //var userModuleAccess = new UserModuleAccessModel
                //{
                //    ProductId = salesDetail.ProductId,
                //    SalesDetailId = salesDetail.Id,
                //    Action = "OUT",
                //    Quantity = salesDetail.Quantity,
                //};

                user = await UpdateAsync(user, userId);
            }
                
            await transaction.CommitAsync();

            return _model;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public async Task<User> SaveNoRollBackAsync(UserModel model, int userId)
    {
        var _model = _mapper.Map<User>(model);

        if (model.Id == 0)
            _model = await CreateAsync(_model, userId);
        else
            _model = await UpdateAsync(_model, userId);

        return _model;
    }
    public async Task<User> CreateAsync(User user, int userId)
    {
        user.DateCreated = DateTime.UtcNow;
        user.CreatedById = userId;

        return await _contextHelper.CreateNoRollbackAsync(user, "ModifiedById",
        "DateModified");
    }
    public async Task<User> UpdateAsync(User user, int userId)
    {
        user.DateModified = DateTime.UtcNow;
        user.ModifiedById = userId;

        return await _contextHelper.UpdateNoRollbackAsync(user, "CreatedById",
        "DateCreated");
    }

    private static string GenerateHashedPassword(string password, byte[] salt)
    {
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashedPassword;
    }

    private async Task ValidateUserAsync(User user)
    {
        if (user.Id == 0)
        {
            var userNameExists = await _context.Users.FirstOrDefaultAsync(e => e.Username == user.Username);
            if (userNameExists is not null)
                throw new Exception("Username already exists!");
        }
        else
        {
            var userNameExists = await _context.Users.FirstOrDefaultAsync(m => m.Id != user.Id && m.Username == user.Username);

            if (userNameExists is not null)
                throw new Exception("Username already exists!");
        }
    }

    private async Task ValidateConfirmPassword(UserModel user)
    {
        if (user.NewPassword != user.ConfirmPassword)
            if (user.Id == 0 && user.NewPassword == "")
                throw new Exception("Password and Confirm password are not the same.");
            else if (user.NewPassword != "" && user.NewPassword != null)
                throw new Exception("New password and Confirm password are not the same.");
    }

    public async Task<User?> GetByUserNameAsync(string userName) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

    public async Task<User?> GetUserByIdAsync(int id) =>
     await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public string GetFirstNameById(int id)
    {
        var name = _context.Users
            .Where(x => x.Id == id)
            .Select(x => x.FirstName)
            .FirstOrDefault();
        return name ?? string.Empty;
    }
}

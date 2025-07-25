using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using InventoryProject.DataAccess.Services;
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
        private readonly InventoryProjectDatabaseContext _context;
        private readonly EfCoreHelper<UserModuleAccess> _contextHelper;
        private readonly ISqlDataAccessService _db;
        //private readonly IUserModuleAccessRepository _userModuleAccessRepository;
        private readonly IMapper _mapper;
        public UserModuleAccessRepository(ISqlDataAccessService db, InventoryProjectDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _contextHelper = new EfCoreHelper<UserModuleAccess>(context);
            _db = db;
            //_userModuleAccessRepository = userModuleAccessRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserModuleAccessModel>> Inq(int? id)
        {
            var data = await _db.LoadData<UserModuleAccessModel, dynamic>(
            "spProductAdjustment_Inq",
            new { id },
            CommandType.StoredProcedure);

            // var _model = _mapper.Map<ProductAdjustmentModel>(data);

            return data;
        }
        public async Task<UserModuleAccess> SaveAsync(UserModuleAccessModel model, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var _model = _mapper.Map<UserModuleAccess>(model);
                int oldQty = 0;

                if (model.Id == 0)
                    _model = await CreateAsync(_model, userId);
                else
                    _model = await UpdateAsync(_model, userId);

                //await _productRepository.UpdateProductQty(model.ProductId, model.Quantity.GetValueOrDefault(), model.Action, oldQty, userId);

                await transaction.CommitAsync();

                return _model;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<UserModuleAccess> SaveNoRollBackAsync(UserModuleAccessModel model, int userId)
        {
            var _model = _mapper.Map<UserModuleAccess>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            return _model;
        }

        public async Task SaveBulkNoRollBackAsync(List<UserModuleAccessModel> modelList, int userId)
        {
            foreach (var model in modelList)
            {
                var entity = _mapper.Map<UserModuleAccess>(model);

                if (model.Id == 0)
                {
                    await CreateAsync(entity, userId);
                }
                else
                {
                    await UpdateAsync(entity, userId); 
                }
            }
        }

        public async Task<List<UserModuleAccess>> SaveBulkAsync(List<UserModuleAccessModel> modelList, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entities = _mapper.Map<List<UserModuleAccess>>(modelList);

                foreach (var entity in entities)
                {
                    UserModuleAccess userModuleAccess;

                    if (entity.Id == 0)
                    {
                        await CreateAsync(entity, userId);
                    }
                    else
                    {                
                        userModuleAccess = await GetUserModuleAccessByIdAsync(entity.Id);
                        userModuleAccess.CanCreate = entity.CanCreate;
                        userModuleAccess.CanDelete = entity.CanDelete;
                        userModuleAccess.CanEdit = entity.CanEdit;
                        userModuleAccess.CanView = entity.CanView;

                        await UpdateAsync(userModuleAccess, userId);
                    }
                }

                await transaction.CommitAsync();

                return entities;
            }

            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<UserModuleAccess> CreateAsync(UserModuleAccess userModuleAccess, int userId)
        {
            userModuleAccess.DateCreated = DateTime.UtcNow;
            userModuleAccess.CreatedById = userId;

            return await _contextHelper.CreateNoRollbackAsync(userModuleAccess, "ModifiedById",
            "DateModified");
        }
        public async Task<UserModuleAccess> UpdateAsync(UserModuleAccess userModuleAccess, int userId)
        {
            userModuleAccess.DateModified = DateTime.UtcNow;
            userModuleAccess.ModifiedById = userId;

            return await _contextHelper.UpdateNoRollbackAsync(userModuleAccess, "CreatedById",
            "DateCreated");
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _contextHelper.GetByIdAsync(id);

            if (product == null)
                return false;

            await _contextHelper.DeleteNoRollbackAsync(product);

            return true;
        }

        public async Task BatchDeleteAsync(List<int> ids)
        {
            var products = await _contextHelper.GetAllAsync();
            var productsToDelete = products.Where(p => ids.Contains(p.Id)).ToList();

            if (!productsToDelete.Any())
                return;

            foreach (var product in productsToDelete)
            {
                await _contextHelper.DeleteNoRollbackAsync(product);
            }
        }

        public async Task BatchDeleteNoRollbackAsync(int[] ids)
        {
            var entities = _context.UserModuleAccesses.Where(d => ids.Contains(d.Id));
            if (entities is not null)
            {
                await _contextHelper.BatchDeleteNoRollbackAsync(entities);
            }
        }

        //public async Task BatchDeleteNoRollbackAsyncBySalesDetailId(int[] ids)
        //{
        //    var entities = _context.UserModuleAccesses.Where(d => d.SalesDetailId.HasValue && ids.Contains(d.SalesDetailId.Value));
        //    if (entities is not null)
        //    {
        //        await _contextHelper.BatchDeleteNoRollbackAsync(entities);
        //    }
        //}

        public async Task<List<UserModuleAccessModel>> GetUserModuleAccess(int userId)
        {
            var data = await _db.LoadData<UserModuleAccessModel, dynamic>(
                "spUserModuleAccess_GetByUserId",
                new { userId },
                CommandType.StoredProcedure);
            return data.ToList();
        }
        public async Task<UserModuleAccess?> GetUserModuleAccessByIdAsync(int id) =>
            await _context.UserModuleAccesses.FirstOrDefaultAsync(u => u.Id == id);
    }
}

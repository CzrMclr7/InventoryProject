using AutoMapper;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo
{
    public class ProductAdjustmentRepository : IProductAdjustmentRepository
    {
        private readonly InventoryProjectDatabaseContext _context;
        private readonly EfCoreHelper<ProductAdjustment> _contextHelper;
        private readonly ISqlDataAccessService _db;     
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductAdjustmentRepository(
                InventoryProjectDatabaseContext context,
                ISqlDataAccessService db,
                IProductRepository productRepository,
                IMapper mapper)
        {
            _context = context;
            _contextHelper = new EfCoreHelper<ProductAdjustment>(context);
            _db = db;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<ProductAdjustmentModel> GetByIdAsync(int id)
        {
            var data = await _db.LoadSingle<ProductAdjustmentModel, dynamic>(
            "spProductAdjustment_Inq",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }

        public async Task<ProductAdjustment> GetById(int id)
        {
            var data = _context.ProductAdjustments.Where(x => x.Id == id).First();

            return data;
        }

        public async Task<IEnumerable<ProductAdjustmentModel>> Inq(int? id)
        {
            var data = await _db.LoadData<ProductAdjustmentModel, dynamic>(
            "spProductAdjustment_Inq",
            new { id },
            CommandType.StoredProcedure);

           // var _model = _mapper.Map<ProductAdjustmentModel>(data);

            return data;
        }
        public async Task<ProductAdjustment> SaveAsync(ProductAdjustmentModel model, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var _model = _mapper.Map<ProductAdjustment>(model);
                int oldQty = 0;

                if (model.Id == 0)
                    _model = await CreateAsync(_model, userId);
                else
                    _model = await UpdateAsync(_model, userId);

                await _productRepository.UpdateProductQty(model.ProductId, model.Quantity.GetValueOrDefault(), model.Action, oldQty, userId);

                await transaction.CommitAsync();

                return _model;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<ProductAdjustment> SaveNoRollBackAsync(ProductAdjustmentModel model, int userId)
        {
            var _model = _mapper.Map<ProductAdjustment>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            return _model;
        }
        public async Task<ProductAdjustment> CreateAsync(ProductAdjustment productAdj, int userId)
        {
            productAdj.DateCreated = DateTime.UtcNow;
            productAdj.CreatedById = userId;

            return await _contextHelper.CreateNoRollbackAsync(productAdj, "ModifiedById",
            "DateModified");
        }
        public async Task<ProductAdjustment> UpdateAsync(ProductAdjustment productAdj, int userId)
        {
            productAdj.DateModified = DateTime.UtcNow;
            productAdj.ModifiedById = userId;

            return await _contextHelper.UpdateNoRollbackAsync(productAdj, "CreatedById",
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
            var entities = _context.ProductAdjustments.Where(d => ids.Contains(d.Id));
            if (entities is not null)
            {
                await _contextHelper.BatchDeleteNoRollbackAsync(entities);
            }
        }

        public async Task BatchDeleteNoRollbackAsyncBySalesDetailId(int[] ids)
        {
            var entities = _context.ProductAdjustments.Where(d => d.SalesDetailId.HasValue && ids.Contains(d.SalesDetailId.Value));
            if (entities is not null)
            {
                await _contextHelper.BatchDeleteNoRollbackAsync(entities);
            }
        }
        public async Task<IEnumerable<SalesModel>> GetProductAdjustmentData(int? id)
        {
            var data = await _db.LoadData<SalesModel, dynamic>(
            "spRpt_ProductAdjustments",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }
    }
}

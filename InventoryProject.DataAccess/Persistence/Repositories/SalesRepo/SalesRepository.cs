using AutoMapper;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;
using InventoryProject.DataAccess.Models.Report;

namespace InventoryProject.DataAccess.Persistence.Repositories.SalesRepo
{
    public class SalesRepository : ISalesRepository
    {
        private readonly InventoryProjectDatabaseContext _context;
        private readonly EfCoreHelper<Sale> _contextHelper;
        private readonly ISqlDataAccessService _db;
        private readonly ISalesDetailRepository _salesDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductAdjustmentRepository _adjustmentRepository;

        public SalesRepository(
            InventoryProjectDatabaseContext context,
            ISqlDataAccessService db,
            IMapper mapper,
            ISalesDetailRepository salesDetailRepository,
            IProductRepository productRepository,
            IProductAdjustmentRepository adjustmentRepository)
        {
            _context = context;
            _contextHelper = new EfCoreHelper<Sale>(context);
            _db = db;
            _mapper = mapper;
            _salesDetailRepository = salesDetailRepository;
            _productRepository = productRepository;
            _adjustmentRepository = adjustmentRepository;
        }

        public async Task<SalesModel> GetByIdAsync(int id)
        {
            var data = await _db.LoadSingle<SalesModel, dynamic>(
            "spSales_Inq",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }

        public async Task<IEnumerable<SalesModel>> Inq(int? id)
        {
            var data = await _db.LoadData<SalesModel, dynamic>(
            "spSales_Inq",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }

        public async Task<Sale> SaveAsync(SalesModel model,
            List<SalesDetailModel> details,
            int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var _model = _mapper.Map<Sale>(model);

                //Saving of Sales Header
                if (model.Id == 0)
                    _model = await CreateAsync(_model, userId);
                else
                    _model = await UpdateAsync(_model, userId);

                var salesDetailToCompare = new List<SalesDetail>();
                var prodAdjustmentToCompare = new List<ProductAdjustment>();


                //Saving Of Sales Details

                if (details.Count() != 0 || !details.Any())
                {
                    foreach (var detail in details)
                    {
                        detail.SalesId = _model.Id;
                        int oldQty = _salesDetailRepository.GetOldSalesDetailQty(detail.SalesId, detail.ProductId);

                        var salesDetail = await _salesDetailRepository.SaveNoRollBackAsync(detail, userId);
                        salesDetailToCompare.Add(salesDetail);                           

                        var productAdjustment = new ProductAdjustmentModel
                        {
                            ProductId = salesDetail.ProductId,
                            SalesDetailId = salesDetail.Id,
                            Action = "OUT",
                            Quantity = salesDetail.Quantity,
                        };

                        await _productRepository.UpdateProductQty(detail.ProductId, detail.Quantity, productAdjustment.Action, oldQty, userId);
                        await _adjustmentRepository.SaveNoRollBackAsync(productAdjustment, userId);
                        
                    }
                }

                var salesDetailsIds = salesDetailToCompare.Where(m => m.Id != 0).Select(m => m.Id).ToList();
                var toDelete = await _context.SalesDetails
                    .Where(m => m.SalesId == model.Id && !salesDetailsIds.Contains(m.Id))
                    .Select(e => e.Id)
                    .ToArrayAsync();

                await _adjustmentRepository.BatchDeleteNoRollbackAsyncBySalesDetailId(toDelete);
                await _salesDetailRepository.BatchDeleteNoRollbackAsync(toDelete);

                await transaction.CommitAsync();

                return _model;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<Sale> SaveNoRollBackAsync(SalesModel model, List<SalesDetailModel> details, ProductModel product, int userId)
        {
            var _model = _mapper.Map<Sale>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            return _model;
        }

        public async Task<Sale> CreateAsync(Sale sales, int userId)
        {
            sales.DateCreated = DateTime.UtcNow;
            sales.CreatedById = userId;

            return await _contextHelper.CreateNoRollbackAsync(sales, "ModifiedById", "DateModified");
        }

        public async Task<Sale> UpdateAsync(Sale sales, int userId)
        {
            sales.DateModified = DateTime.UtcNow;
            sales.ModifiedById = userId;

            return await _contextHelper.UpdateNoRollbackAsync(sales, "CreatedById",
            "DateCreated");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sales = await _contextHelper.GetByIdAsync(id);

            if (sales == null)
                return false;

            await _contextHelper.DeleteNoRollbackAsync(sales);

            return true;
        }

        public async Task BatchDeleteAsync(List<int> ids)
        {
            var sales = await _contextHelper.GetAllAsync();
            var salesToDelete = sales.Where(p => ids.Contains(p.Id)).ToList();

            if (!salesToDelete.Any())
                return;

            foreach (var sale in salesToDelete)
            {
                await _contextHelper.DeleteNoRollbackAsync(sale);
            }
        }
        public async Task<IEnumerable<SalesModel>> GetDetailedSalesData(int? id)
        {
            var data = await _db.LoadData<SalesModel, dynamic>(
            "spRpt_DetailedSales",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }
    }
}
using AutoMapper;
using DevExpress.ClipboardSource.SpreadsheetML;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo
{
    public class SalesDetailRepository : ISalesDetailRepository
    {
        private readonly InventoryProjectDatabaseContext _context;
        private readonly EfCoreHelper<SalesDetail> _contextHelper;
        private readonly ISqlDataAccessService _db;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public SalesDetailRepository(
                InventoryProjectDatabaseContext context,
                ISqlDataAccessService db,
                IMapper mapper,
                IProductRepository productRepository)
        {
            _context = context;
            _contextHelper = new EfCoreHelper<SalesDetail>(context);
            _db = db;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<SalesDetailModel> GetByIdAsync(int id)
        {
            var data = await _db.LoadSingle<SalesDetailModel, dynamic>(
            "spSalesDetail_GetBySalesId",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }

        public async Task<IEnumerable<SalesDetailModel>> Inq(int? id)
        {
            var data = await _db.LoadData<SalesDetailModel, dynamic>(
            "spSalesDetail_GetBySalesId",
            new { id },
            CommandType.StoredProcedure);
            return data;
        }

        public async Task<SalesDetail> SaveAsync(SalesDetailModel model, int userId)
        {
            var _model = _mapper.Map<SalesDetail>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            return _model;
        }

        public async Task<SalesDetail> SaveNoRollBackAsync(SalesDetailModel model, int userId)
        {
            var _model = _mapper.Map<SalesDetail>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            return _model;
        }

        public async Task<SalesDetail> CreateAsync(SalesDetail salesDetail, int userId)
        {
            salesDetail.DateCreated = DateTime.UtcNow;
            salesDetail.CreatedById = userId;

            return await _contextHelper.CreateNoRollbackAsync(salesDetail, "ModifiedById",
            "DateModified");
        }

        public async Task<SalesDetail> UpdateAsync(SalesDetail salesDetail, int userId)
        {
            salesDetail.DateModified = DateTime.UtcNow;
            salesDetail.ModifiedById = userId;

            return await _contextHelper.UpdateNoRollbackAsync(salesDetail, "CreatedById",
            "DateCreated");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var salesDetail = await _contextHelper.GetByIdAsync(id);

            if (salesDetail == null)
                return false;

            await _contextHelper.DeleteNoRollbackAsync(salesDetail);

            return true;
        }

        public async Task BatchDeleteAsync(List<int> ids)
        {
            var salesDetails = await _contextHelper.GetAllAsync();
            var salesDetailsToDelete = salesDetails.Where(p => ids.Contains(p.Id)).ToList();

            if (!salesDetailsToDelete.Any())
                return;

            foreach (var salesDetail in salesDetailsToDelete)
            {
                await _contextHelper.DeleteNoRollbackAsync(salesDetail);
            }
        }

        public async Task<SalesDetailModel> GetBySalesIdA(int id)
        {
            var data = await _db.LoadData<SalesDetailModel, dynamic>(
            "spSalesDetail_GetBySalesId",
            new { id },
            CommandType.StoredProcedure);
            return (SalesDetailModel)data;
        }

        public async Task BatchDeleteNoRollbackAsync(int[] ids)
        {
            var entities = _context.SalesDetails.Where(d => ids.Contains(d.Id));
            if (entities is not null)
            {
                await _contextHelper.BatchDeleteNoRollbackAsync(entities);
            }
        }

        public int GetOldSalesDetailQty(int salesId, int productId)
        {
            var qty = _context.SalesDetails.Where(x => x.SalesId == salesId && x.ProductId == productId)
                .Select(x => x.Quantity)
                .FirstOrDefault();
            //var _data = _mapper.Map<SalesDetailModel>(data);
            return qty;
        }
    }
}
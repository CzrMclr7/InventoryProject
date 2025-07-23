using AutoMapper;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Services;
using System.Data;
namespace InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
public class ProductRepository : IProductRepository
{
    private readonly InventoryProjectDatabaseContext _context;
    private readonly EfCoreHelper<Product> _contextHelper;
    private readonly ISqlDataAccessService _db;
    private readonly IMapper _mapper;

    public ProductRepository(
            InventoryProjectDatabaseContext context,
            ISqlDataAccessService db,
            IMapper mapper)
    {
        _context = context;
        _contextHelper = new EfCoreHelper<Product>(context);
        _db = db;
        _mapper = mapper;
    }
    public async Task<ProductModel> GetByIdAsync(int id)
    {
        var data = await _db.LoadSingle<ProductModel, dynamic>(
        "spProducts_Inq",
        new { id },
        CommandType.StoredProcedure);
        return data;
    }

    public async Task<Product> GetById(int id)
    {
        var data = _context.Products.Where(x => x.Id == id).First();

        return data;
    }

    public string GetProductNameById(int id)
    {
        var data = this.GetByIdAsync(id).Result.Name.ToString();

        return data;
    }

    public async Task<IEnumerable<ProductModel>> Inq(int? id)
    {
        var data = await _db.LoadData<ProductModel, dynamic>(
        "spProducts_Inq",
        new { id },
        CommandType.StoredProcedure);
        return data;
    }
    public async Task<Product> SaveAsync(ProductModel model, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var _model = _mapper.Map<Product>(model);

            if (model.Id == 0)
                _model = await CreateAsync(_model, userId);
            else
                _model = await UpdateAsync(_model, userId);

            await transaction.CommitAsync();

            return _model;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            throw;
        }
    } 

 public async Task<Product> SaveNoRollBackAsync(ProductModel model, int userId)
    {
        var _model = _mapper.Map<Product>(model);

        if (model.Id == 0)
            _model = await CreateAsync(_model, userId);
        else
            _model = await UpdateAsync(_model, userId);

        return _model;
    }
    public async Task<Product> CreateAsync(Product product, int userId)
    {
        product.DateCreated = DateTime.UtcNow;
        product.CreatedById = userId;

        return await _contextHelper.CreateNoRollbackAsync(product, "ModifiedById",
        "DateModified");
    }
    public async Task<Product> UpdateAsync(Product product, int userId)
    {
        product.DateModified = DateTime.UtcNow;
        product.ModifiedById = userId;

        return await _contextHelper.UpdateNoRollbackAsync(product, "CreatedById",
        "DateCreated");
    }

    public async Task<Product> UpdateProductQty(int productId, int qtyChange, string action, int? oldQty, int userId)
    {
        var product = await GetById(productId);

        if (product == null)
            throw new Exception($"Product with ID {productId} not found.");

        if (action == "OUT")
        {
            if (oldQty.HasValue && oldQty.Value > 0)
                product.Qty += oldQty.Value;

            product.Qty -= qtyChange;
        }
        else if (action == "IN")
        {
            if (oldQty.HasValue && oldQty.Value > 0)
                product.Qty -= oldQty.Value;

            product.Qty += qtyChange;
        }

        product.ModifiedById = userId;

        return await _contextHelper.UpdateNoRollbackAsync(product, "CreatedById", "DateCreated");
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
}
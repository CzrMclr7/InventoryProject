using InventoryProject.DataAccess.Models;

namespace InventoryProject.App.ViewModels;

public class ProductAdjustmentViewModel
{
    public ProductAdjustmentModel? ProductAdjustmentModel { get; set; } //Header
    //public List<>? SalesDetails { get; set; } //Details
    public List<ProductAdjustmentModel>? ProductAdjustments { get; set; }
    public ProductModel? Products { get; set; }
    public UserModuleAccessModel AccessRights { get; set; }


}

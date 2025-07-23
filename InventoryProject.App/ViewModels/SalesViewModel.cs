using InventoryProject.DataAccess.Models;

namespace InventoryProject.App.ViewModels;

public class SalesViewModel
{
    public SalesModel? Sales { get; set; } //Header
    public List<SalesDetailModel>? SalesDetails { get; set; } //Details
    public ProductModel? Products { get; set; } 
}
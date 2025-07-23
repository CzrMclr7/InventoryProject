namespace InventoryProject.DataAccess.Models;

public class SalesModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalPrice { get; set; }
    public int Quantity { get; set; }
    public DateTime DateCreated { get; set; }
    public int CreatedById { get; set; }
    public DateTime? DateModified { get; set; }
    public int? ModifiedById { get; set; }

    #region Report

    public string? SalesName { get; set; }
    public string? ProductName { get; set; }
    //public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    #endregion
}
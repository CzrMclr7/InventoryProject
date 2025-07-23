using System.ComponentModel.DataAnnotations;

namespace InventoryProject.DataAccess.Models;

public class SalesDetailModel
{
    public int Id { get; set; }
    public int SalesId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
    public int Quantity { get; set; }
    public DateTime DateCreated { get; set; }
    public int CreatedById { get; set; }
    public DateTime? DateModified { get; set; }
    public int? ModifiedById { get; set; }
    public string? Name { get; set; }
}
using InventoryProject.DataAccess.DataContextModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Models
{
    public class ProductAdjustmentModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Action { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int? Quantity { get; set; }

        public int? SalesDetailId { get; set; }
        public DateTime DateCreated { get; set; }

        public int CreatedById { get; set; }

        public DateTime? DateModified { get; set; }

        public int? ModifiedById { get; set; }
        public virtual Product Product { get; set; }

        public virtual SalesDetail SalesDetail { get; set; }
        public string? Name { get; set; }
        public string DateCreatedFormatted => DateCreated.ToString("MM/dd/yyyy");
    }
}

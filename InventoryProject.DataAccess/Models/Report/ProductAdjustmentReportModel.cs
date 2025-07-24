using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Models.Report
{
    public class ProductAdjustmentReportModel
    {
        public string Company { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public List<ProductAdjustmentModel> ProductAdjustments { get; set; }
    }
}

using InventoryProject.DataAccess.DataContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Models
{
    public class ModuleModel
    {
        public int Id { get; set; }

        public string ModuleName { get; set; }

        public string ModuleCode { get; set; }

        public virtual ICollection<UserModuleAccess> UserModuleAccesses { get; set; } = new List<UserModuleAccess>();
    }
}

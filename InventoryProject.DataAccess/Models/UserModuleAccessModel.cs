using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Models
{
    public class UserModuleAccessModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ModuleId { get; set; }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanView { get; set; }

        public int CreatedById { get; set; }

        public DateTime DateCreated { get; set; }

        public int? ModifiedById { get; set; }

        public DateTime? DateModified { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
    }
}

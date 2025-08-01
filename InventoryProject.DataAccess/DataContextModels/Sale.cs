﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace InventoryProject.DataAccess.DataContextModels;

public partial class Sale
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal TotalPrice { get; set; }

    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedById { get; set; }

    public DateTime? DateModified { get; set; }

    public int? ModifiedById { get; set; }

    public virtual ICollection<SalesDetail> SalesDetails { get; set; } = new List<SalesDetail>();
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFAShopMgt23.Model;

public partial class Product: ICloneable, INotifyPropertyChanged
{
    public int Id { get; set; }

    public int? CatId { get; set; }

    public string? Name { get; set; }

    public string? Price { get; set; }

    public int? Qty { get; set; }

    public string? ImagePath { get; set; }

    public int? PriceInt { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public object Clone()
    {
        return MemberwiseClone();
    }
}

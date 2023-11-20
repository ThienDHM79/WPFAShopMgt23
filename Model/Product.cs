using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFAShopMgt23;

public partial class Product: INotifyPropertyChanged, ICloneable
{
    public int Id { get; set; }

    public int? CatId { get; set; }

    public string? Name { get; set; }

    public string? Price { get; set; }

    public int? Qty { get; set; }

    public string? ImagePath { get; set; }

    public int? PriceInt { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public object Clone()
    {
        return MemberwiseClone();
    }
}

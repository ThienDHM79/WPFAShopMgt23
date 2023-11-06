using System;
using System.Collections.Generic;

namespace WPFAShopMgt23;

public partial class PurchaseDetail
{
    public int Id { get; set; }

    public int? PurchaseId { get; set; }

    public int? ProductId { get; set; }

    public double? Price { get; set; }

    public int? Qty { get; set; }

    public int? Total { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Purchase? Purchase { get; set; }
}

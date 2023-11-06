using System;
using System.Collections.Generic;

namespace WPFAShopMgt23;

public partial class Purchase
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public double? FinalTotal { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
}

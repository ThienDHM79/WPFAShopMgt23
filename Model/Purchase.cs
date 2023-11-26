using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFAShopMgt23.Model;

public partial class Purchase: ICloneable, INotifyPropertyChanged
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public double? FinalTotal { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public object Clone()
    {
        return MemberwiseClone();
    }
}

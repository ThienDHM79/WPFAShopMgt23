using System;
using System.Collections.Generic;

namespace WPFAShopMgt23;

public partial class Customer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Tel { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

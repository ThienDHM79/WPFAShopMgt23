using System;
using System.Collections.Generic;

namespace WPFAShopMgt23.Model;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

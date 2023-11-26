using System;
using System.Collections.Generic;

namespace WPFAShopMgt23.Model;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string? Rolename { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

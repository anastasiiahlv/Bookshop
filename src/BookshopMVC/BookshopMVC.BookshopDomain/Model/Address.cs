using System;
using System.Collections.Generic;

namespace BookshopDomain.Model;

public partial class Address: Entity
{
    public int CountryId { get; set; }

    public string Address1 { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

using System;
using System.Collections.Generic;

namespace BookshopDomain.Model;

public partial class OrdersBook: Entity
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int BookQuantity { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}

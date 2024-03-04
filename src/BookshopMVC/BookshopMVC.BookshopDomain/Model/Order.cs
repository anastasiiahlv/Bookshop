using System;
using System.Collections.Generic;

namespace BookshopDomain.Model;

public partial class Order: Entity
{
    public int CustomerId { get; set; }

    public int AddressId { get; set; }

    public int StatusId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly DispatchDate { get; set; }

    public DateOnly ArrivalDate { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrdersBook> OrdersBooks { get; set; } = new List<OrdersBook>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}

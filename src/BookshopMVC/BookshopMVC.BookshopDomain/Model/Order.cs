using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Order: Entity
{
    public Order()
    {
        OrdersBooks = new HashSet<OrdersBook>();
    }

    [Display(Name = "Користувач")]
    public int CustomerId { get; set; }

    [Display(Name = "Адреса доставки")]
    public int AddressId { get; set; }

    [Display(Name = "Статус замовлення")]
    public int StatusId { get; set; }

    [Display(Name = "Спосіб оплати")]
    public int PaymentMethodId { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime CreationDate { get; set; }

    [Display(Name = "Дата відправлення")]
    public DateTime DispatchDate { get; set; }

    [Display(Name = "Дата прибуття")]
    public DateTime ArrivalDate { get; set; }

    [Display(Name = "Адреса доставки")]
    public virtual Address Address { get; set; } = null!;

    [Display(Name = "Користувач")]
    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrdersBook> OrdersBooks { get; set; } = new List<OrdersBook>();

    [Display(Name = "Спосіб оплати")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    [Display(Name = "Статус замовлення")]
    public virtual Status Status { get; set; } = null!;
}

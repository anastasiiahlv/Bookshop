using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using System.IO;
using System.Security.Policy;
using System.ComponentModel.DataAnnotations;

namespace BookshopInfrastructure.Models
{
    public class OrderModel
    {
        [Display(Name = "Користувач")]
        public int CustomerId { get; set; }

        [Display(Name = "Адреса доставки")]
        public int AddressId { get; set; }

        [Display(Name = "Статус замовлення")]
        public int StatusId { get; set; }

        [Display(Name = "Спосіб оплати")]
        public int PaymentMethodId { get; set; }
        public List<OrderItemModel>? OrderItems { get; set; }
    }
}

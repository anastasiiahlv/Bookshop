using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using System.IO;
using System.Security.Policy;

namespace BookshopInfrastructure.Models
{
    public class OrderItemModel
    {
        public int BookId { get; set; }
        public int BookQuantity { get; set; }
    }
}

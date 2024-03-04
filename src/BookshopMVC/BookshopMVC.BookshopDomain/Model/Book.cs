using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Book: Entity
{ 
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(100, ErrorMessage = "Назва не має перевищувати 100 символів")]
    public string Title { get; set; } = null!;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    public string Description { get; set; } = null!;

    [Display(Name = "Ціна")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [Range(0.01, 20000.00, ErrorMessage = "Ціна повинна бути більше 0 та менше за 20000")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Неправильний формат")]
    [DataType(DataType.Currency)]
    public double Price { get; set; }
    public virtual Publisher Publisher { get; set; } = null!;
    public int PublisherId { get; set; }
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<OrdersBook> OrdersBooks { get; set; } = new List<OrdersBook>();
}

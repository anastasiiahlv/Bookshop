using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Address: Entity
{
    [Display(Name = "Країна доставки")]
    public int CountryId { get; set; }

    [Display(Name = "Адреса")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [RegularExpression(@"^м\.\s[a-zA-Zа-яА-ЯїЇіІєЄ'’`ʼ]+,\sвул\.\s[a-zA-Zа-яА-ЯїЇіІєЄ'’`ʼ]+\sбуд\.\s\d+([,-]?\s?(кв\.)?\s?\d+)?$",
    ErrorMessage = "Неправильний формат.")]
    public string Address1 { get; set; } = null!;

    [Display(Name = "Країна доставки")]
    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

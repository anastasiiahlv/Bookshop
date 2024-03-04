using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Country: Entity
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Назва не має перевищувати 50 символів")]
    [RegularExpression(@"^[a-zA-Zа-яА-ЯІіЇїЄє']+$", ErrorMessage = "У назві дозволені лише літери та апостроф")]
    public string? Name { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}

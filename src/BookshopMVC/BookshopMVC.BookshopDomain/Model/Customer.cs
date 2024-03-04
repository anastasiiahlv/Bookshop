using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Customer: Entity
{
    [Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Ім'я не має перевищувати 50 символів")]
    [RegularExpression(@"^[a-zA-Zа-яА-ЯІіЇїЄє']+$", ErrorMessage = "В імені дозволені лише літери та апостроф")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Прізвище")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Прізвище не має перевищувати 50 символів")]
    [RegularExpression(@"^[a-zA-Zа-яА-ЯІіЇїЄє']+$", ErrorMessage = "У прізвищі дозволені лише літери та апостроф")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Адреса електронної пошти")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Потрібний формат email: example@example.com")]
    public string Email { get; set; } = null!;

    [Display(Name = "Номер телефону")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Потрібний формат номеру телефону: +380XXXXXXXXX")]
    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

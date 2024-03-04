using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Author: Entity
{
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

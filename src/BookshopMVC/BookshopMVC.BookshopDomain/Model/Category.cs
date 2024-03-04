﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshopDomain.Model;

public partial class Category: Entity
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(30, ErrorMessage = "Назва не має перевищувати 30 символів")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

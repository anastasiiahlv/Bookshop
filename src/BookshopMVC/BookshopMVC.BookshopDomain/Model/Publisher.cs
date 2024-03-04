using System;
using System.Collections.Generic;

namespace BookshopDomain.Model;

public partial class Publisher: Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

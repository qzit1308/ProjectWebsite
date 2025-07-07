using System;
using System.Collections.Generic;

namespace ProjectWebsite.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string AuthorName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

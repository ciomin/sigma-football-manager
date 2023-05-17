using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class Administrator
{
    public int AdminId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<News> News { get; set; } = new List<News>();
}

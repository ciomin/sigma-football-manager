using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Administrator
{
    public int AdminId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<News> News { get; set; } = new List<News>();
}

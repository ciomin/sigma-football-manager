using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class News
{
    public int NewsId { get; set; }

    public int AdminId { get; set; }

    public string Post { get; set; } = null!;

    public virtual Administrator Admin { get; set; } = null!;
}

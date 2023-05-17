using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Award
{
    public int AwardId { get; set; }

    public string NameOfAward { get; set; } = null!;

    public int PlayerId { get; set; }

    public string Season { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}

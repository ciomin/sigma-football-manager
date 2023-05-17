using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class Stadium
{
    public int StadiumId { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int? Capacity { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}

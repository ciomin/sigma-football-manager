using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class League
{
    public int LeagueId { get; set; }

    public string LeagueName { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}

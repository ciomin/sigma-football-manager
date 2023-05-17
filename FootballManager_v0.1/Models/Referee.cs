using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Referee
{
    public int RefereeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public int? Age { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}

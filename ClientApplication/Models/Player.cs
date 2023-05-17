using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int OverallRank { get; set; }

    public int? Attacking { get; set; }

    public int? MidfieldControl { get; set; }

    public int? Defending { get; set; }

    public string Team { get; set; } = null!;

    public int LeagueId { get; set; }

    public int? Age { get; set; }

    public string Country { get; set; } = null!;

    public string? Path { get; set; }

    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();

    public virtual League League { get; set; } = null!;

    public virtual ICollection<Scorer> Scorers { get; set; } = new List<Scorer>();

    public virtual ICollection<TeamContract> TeamContracts { get; set; } = new List<TeamContract>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}

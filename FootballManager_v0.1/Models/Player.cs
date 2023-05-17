using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FootballManager_v0._1.Models;

public partial class Player
{
    public int PlayerId { get; set; }
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    public string Position { get; set; } = null!;
    [Display(Name = "Overall")]
    public int OverallRank { get; set; }
    [Display(Name = "Att")]
    public int? Attacking { get; set; }
    [Display(Name = "Mid")]
    public int? MidfieldControl { get; set; }
    [Display(Name = "Def")]
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

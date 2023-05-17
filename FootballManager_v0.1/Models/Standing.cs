using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Standing
{
    public int StandingsId { get; set; }

    public int UserId { get; set; }

    public int Position { get; set; }

    public int MatchesPlayed { get; set; }

    public int MatchesWon { get; set; }

    public int MatchesDrawn { get; set; }

    public int GoalsFor { get; set; }

    public int GoalsAgainst { get; set; }

    public int Points { get; set; }

    public virtual User User { get; set; } = null!;
}

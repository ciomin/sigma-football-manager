using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int HomeTeamId { get; set; }

    public int AwayTeamId { get; set; }

    public int HomeGoals { get; set; }

    public int AwayGoals { get; set; }

    public int StadiumId { get; set; }

    public int? RefereeId { get; set; }

    public virtual User AwayTeam { get; set; } = null!;

    public virtual User HomeTeam { get; set; } = null!;

    public virtual Referee? Referee { get; set; }

    public virtual Stadium Stadium { get; set; } = null!;
}

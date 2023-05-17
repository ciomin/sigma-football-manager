using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class TeamContract
{
    public int ContractId { get; set; }

    public int SquadId { get; set; }

    public int PlayerId { get; set; }

    public int ShirtNumber { get; set; }

    public bool IsCaptain { get; set; }

    public bool? IsFirstTeam { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Squad Squad { get; set; } = null!;
}

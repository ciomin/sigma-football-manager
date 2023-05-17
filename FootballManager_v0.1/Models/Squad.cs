using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Squad
{
    public int SquadId { get; set; }

    public int UserId { get; set; }

    public string SquadName { get; set; } = null!;

    public virtual ICollection<TeamContract> TeamContracts { get; set; } = new List<TeamContract>();

    public virtual User User { get; set; } = null!;
}

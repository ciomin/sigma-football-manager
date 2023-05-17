using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Formation
{
    public int FormationId { get; set; }

    public string Formations { get; set; } = null!;

    public string Tactics { get; set; } = null!;
}

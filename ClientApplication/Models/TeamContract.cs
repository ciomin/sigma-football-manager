using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class TeamContract
{
    public int ContractId { get; set; }

    public int? SquadId { get; set; }

    public int? PlayerId { get; set; }

    public int? ShirtNumber { get; set; }

    public bool? IsCaptain { get; set; }

    public int? Position { get; set; }

    public virtual Player? Player { get; set; }

    public virtual Squad? Squad { get; set; }
}

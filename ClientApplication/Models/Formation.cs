using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class Formation
{
    public int FormationId { get; set; }

    public string Formations { get; set; } = null!;

    public string Tactics { get; set; } = null!;
}

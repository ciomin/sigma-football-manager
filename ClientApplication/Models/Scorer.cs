using System;
using System.Collections.Generic;

namespace ClientApplication.Models;

public partial class Scorer
{
    public int ScorerId { get; set; }

    public int PlayerId { get; set; }

    public int NumberOfGoals { get; set; }

    public int NumberOfAssists { get; set; }

    public virtual Player Player { get; set; } = null!;
}

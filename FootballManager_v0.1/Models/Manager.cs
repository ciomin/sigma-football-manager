using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public int? YearsOfExperience { get; set; }
}

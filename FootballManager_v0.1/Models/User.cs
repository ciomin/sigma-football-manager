using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FootballManager_v0._1.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;
    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public int Coins { get; set; }
    [Display(Name = "Team Name")]
    public string NameOfTeam { get; set; } = null!;

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<Squad> Squads { get; set; } = new List<Squad>();

    public virtual ICollection<Standing> Standings { get; set; } = new List<Standing>();

    public virtual ICollection<Transfer> TransferBuyingUsers { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferSellingUsers { get; set; } = new List<Transfer>();
}

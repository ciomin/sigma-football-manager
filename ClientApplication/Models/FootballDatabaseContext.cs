using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClientApplication.Models;

public partial class FootballDatabaseContext : DbContext
{
    public FootballDatabaseContext()
    {
    }

    public FootballDatabaseContext(DbContextOptions<FootballDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Award> Awards { get; set; }

    public virtual DbSet<Formation> Formations { get; set; }

    public virtual DbSet<League> Leagues { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Referee> Referees { get; set; }

    public virtual DbSet<Scorer> Scorers { get; set; }

    public virtual DbSet<Squad> Squads { get; set; }

    public virtual DbSet<Stadium> Stadiums { get; set; }

    public virtual DbSet<Standing> Standings { get; set; }

    public virtual DbSet<TeamContract> TeamContracts { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-R13HREC;Database=Football_database;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Romanian_CI_AS");

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdminId);

            entity.Property(e => e.AdminId)
                .ValueGeneratedNever()
                .HasColumnName("admin_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Award>(entity =>
        {
            entity.Property(e => e.AwardId)
                .ValueGeneratedNever()
                .HasColumnName("award_id");
            entity.Property(e => e.NameOfAward)
                .HasMaxLength(30)
                .HasColumnName("name_of_award");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Season)
                .HasMaxLength(10)
                .HasColumnName("season");

            entity.HasOne(d => d.Player).WithMany(p => p.Awards)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Awards_Players");
        });

        modelBuilder.Entity<Formation>(entity =>
        {
            entity.Property(e => e.FormationId)
                .ValueGeneratedNever()
                .HasColumnName("formation_id");
            entity.Property(e => e.Formations)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("formations");
            entity.Property(e => e.Tactics)
                .HasMaxLength(30)
                .HasColumnName("tactics");
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.Property(e => e.LeagueId)
                .ValueGeneratedNever()
                .HasColumnName("league_id");
            entity.Property(e => e.Country)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("country");
            entity.Property(e => e.LeagueName)
                .HasMaxLength(50)
                .HasColumnName("league_name");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.Property(e => e.ManagerId)
                .ValueGeneratedNever()
                .HasColumnName("manager_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasColumnName("last_name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(30)
                .HasColumnName("nationality");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(e => e.MatchId)
                .ValueGeneratedNever()
                .HasColumnName("match_id");
            entity.Property(e => e.AwayGoals).HasColumnName("away_goals");
            entity.Property(e => e.AwayTeamId).HasColumnName("away_team_id");
            entity.Property(e => e.HomeGoals).HasColumnName("home_goals");
            entity.Property(e => e.HomeTeamId).HasColumnName("home_team_id");
            entity.Property(e => e.RefereeId).HasColumnName("referee_id");
            entity.Property(e => e.StadiumId).HasColumnName("stadium_id");
            entity.Property(e => e.AwayMentality).HasColumnName("away_mentality");
            entity.Property(e => e.HomeMentality).HasColumnName("home_mentality");

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_Users_Awayteam");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_Users_Hometeam");

            entity.HasOne(d => d.Referee).WithMany(p => p.Matches)
                .HasForeignKey(d => d.RefereeId)
                .HasConstraintName("FK_Matches_Referees");

            entity.HasOne(d => d.Stadium).WithMany(p => p.Matches)
                .HasForeignKey(d => d.StadiumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_Stadiums");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.NewsId)
                .ValueGeneratedNever()
                .HasColumnName("news_id");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Post)
                .HasColumnType("text")
                .HasColumnName("post");

            entity.HasOne(d => d.Admin).WithMany(p => p.News)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_Administrators");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(e => e.PlayerId)
                .ValueGeneratedNever()
                .HasColumnName("player_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Attacking).HasColumnName("attacking");
            entity.Property(e => e.Country)
                .HasMaxLength(40)
                .HasColumnName("country");
            entity.Property(e => e.Defending).HasColumnName("defending");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasColumnName("last_name");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.MidfieldControl).HasColumnName("midfield_control");
            entity.Property(e => e.OverallRank).HasColumnName("overall_rank");
            entity.Property(e => e.Path)
                .HasMaxLength(60)
                .HasColumnName("path");
            entity.Property(e => e.Position)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("position");
            entity.Property(e => e.Team)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("team");

            entity.HasOne(d => d.League).WithMany(p => p.Players)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Players_Leagues");
        });

        modelBuilder.Entity<Referee>(entity =>
        {
            entity.Property(e => e.RefereeId)
                .ValueGeneratedNever()
                .HasColumnName("referee_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasColumnName("last_name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(30)
                .HasColumnName("nationality");
        });

        modelBuilder.Entity<Scorer>(entity =>
        {
            entity.Property(e => e.ScorerId)
                .ValueGeneratedNever()
                .HasColumnName("scorer_id");
            entity.Property(e => e.NumberOfAssists).HasColumnName("number_of_assists");
            entity.Property(e => e.NumberOfGoals).HasColumnName("number_of_goals");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");

            entity.HasOne(d => d.Player).WithMany(p => p.Scorers)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scorers_Players");
        });

        modelBuilder.Entity<Squad>(entity =>
        {
            entity.Property(e => e.SquadId)
                .ValueGeneratedNever()
                .HasColumnName("squad_id");
            entity.Property(e => e.SquadName)
                .HasMaxLength(50)
                .HasColumnName("squad_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Squads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Squads_Users");
        });

        modelBuilder.Entity<Stadium>(entity =>
        {
            entity.Property(e => e.StadiumId)
                .ValueGeneratedNever()
                .HasColumnName("stadium_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.City)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("country");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Standing>(entity =>
        {
            entity.HasKey(e => e.StandingsId);

            entity.Property(e => e.StandingsId)
                .ValueGeneratedNever()
                .HasColumnName("standings_id");
            entity.Property(e => e.GoalsAgainst).HasColumnName("goals_against");
            entity.Property(e => e.GoalsFor).HasColumnName("goals_for");
            entity.Property(e => e.MatchesDrawn).HasColumnName("matches_drawn");
            entity.Property(e => e.MatchesPlayed).HasColumnName("matches_played");
            entity.Property(e => e.MatchesLost).HasColumnName("matches_lost");
            entity.Property(e => e.MatchesWon).HasColumnName("matches_won");
            entity.Property(e => e.Trophies).HasColumnName("trophies");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Standings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Standings_Users");
        });

        modelBuilder.Entity<TeamContract>(entity =>
        {
            entity.HasKey(e => e.ContractId);

            entity.Property(e => e.ContractId)
                .ValueGeneratedNever()
                .HasColumnName("contract_id");
            entity.Property(e => e.IsCaptain).HasColumnName("is_captain");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.ShirtNumber).HasColumnName("shirt_number");
            entity.Property(e => e.SquadId).HasColumnName("squad_id");

            entity.HasOne(d => d.Player).WithMany(p => p.TeamContracts)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_Squad_players_Players");

            entity.HasOne(d => d.Squad).WithMany(p => p.TeamContracts)
                .HasForeignKey(d => d.SquadId)
                .HasConstraintName("FK_Squad_players_Squads1");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.Property(e => e.TransferId)
                .ValueGeneratedNever()
                .HasColumnName("transfer_id");
            entity.Property(e => e.BuyingUserId).HasColumnName("buying_user_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.SellingUserId).HasColumnName("selling_user_id");
            entity.Property(e => e.TransferDate)
                .HasColumnType("date")
                .HasColumnName("transfer_date");
            entity.Property(e => e.TransferFee).HasColumnName("transfer_fee");

            entity.HasOne(d => d.BuyingUser).WithMany(p => p.TransferBuyingUsers)
                .HasForeignKey(d => d.BuyingUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Users_Buying");

            entity.HasOne(d => d.Player).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Players");

            entity.HasOne(d => d.SellingUser).WithMany(p => p.TransferSellingUsers)
                .HasForeignKey(d => d.SellingUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_Users_Selling");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Coins).HasColumnName("coins");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("date")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.NameOfTeam)
                .HasMaxLength(50)
                .HasColumnName("name_of_team");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

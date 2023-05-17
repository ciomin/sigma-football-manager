using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientApplication.Migrations
{
    /// <inheritdoc />
    public partial class MyFirstUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "Formations",
                columns: table => new
                {
                    formation_id = table.Column<int>(type: "int", nullable: false),
                    formations = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    tactics = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formations", x => x.formation_id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    league_id = table.Column<int>(type: "int", nullable: false),
                    league_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    country = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.league_id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nationality = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    years_of_experience = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.manager_id);
                });

            migrationBuilder.CreateTable(
                name: "Referees",
                columns: table => new
                {
                    referee_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nationality = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    age = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referees", x => x.referee_id);
                });

            migrationBuilder.CreateTable(
                name: "Stadiums",
                columns: table => new
                {
                    stadium_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    city = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: false),
                    country = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stadiums", x => x.stadium_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: true),
                    coins = table.Column<int>(type: "int", nullable: false),
                    name_of_team = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    news_id = table.Column<int>(type: "int", nullable: false),
                    admin_id = table.Column<int>(type: "int", nullable: false),
                    post = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.news_id);
                    table.ForeignKey(
                        name: "FK_News_Administrators",
                        column: x => x.admin_id,
                        principalTable: "Administrators",
                        principalColumn: "admin_id");
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    position = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: false),
                    overall_rank = table.Column<int>(type: "int", nullable: false),
                    attacking = table.Column<int>(type: "int", nullable: false),
                    midfield_control = table.Column<int>(type: "int", nullable: false),
                    defending = table.Column<int>(type: "int", nullable: false),
                    team = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: false),
                    league_id = table.Column<int>(type: "int", nullable: false),
                    age = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.player_id);
                    table.ForeignKey(
                        name: "FK_Players_Leagues",
                        column: x => x.league_id,
                        principalTable: "Leagues",
                        principalColumn: "league_id");
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    match_id = table.Column<int>(type: "int", nullable: false),
                    home_team_id = table.Column<int>(type: "int", nullable: false),
                    away_team_id = table.Column<int>(type: "int", nullable: false),
                    score = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    stadium_id = table.Column<int>(type: "int", nullable: false),
                    referee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_Matches_Referees",
                        column: x => x.referee_id,
                        principalTable: "Referees",
                        principalColumn: "referee_id");
                    table.ForeignKey(
                        name: "FK_Matches_Stadiums",
                        column: x => x.stadium_id,
                        principalTable: "Stadiums",
                        principalColumn: "stadium_id");
                    table.ForeignKey(
                        name: "FK_Matches_Users_Awayteam",
                        column: x => x.away_team_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Matches_Users_Hometeam",
                        column: x => x.home_team_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Squads",
                columns: table => new
                {
                    squad_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    squad_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Squads", x => x.squad_id);
                    table.ForeignKey(
                        name: "FK_Squads_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    standings_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    position = table.Column<int>(type: "int", nullable: false),
                    matches_played = table.Column<int>(type: "int", nullable: false),
                    matches_won = table.Column<int>(type: "int", nullable: false),
                    matches_drawn = table.Column<int>(type: "int", nullable: false),
                    goals_for = table.Column<int>(type: "int", nullable: false),
                    goals_against = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => x.standings_id);
                    table.ForeignKey(
                        name: "FK_Standings_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    award_id = table.Column<int>(type: "int", nullable: false),
                    name_of_award = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    season = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.award_id);
                    table.ForeignKey(
                        name: "FK_Awards_Players",
                        column: x => x.player_id,
                        principalTable: "Players",
                        principalColumn: "player_id");
                });

            migrationBuilder.CreateTable(
                name: "Scorers",
                columns: table => new
                {
                    scorer_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    number_of_goals = table.Column<int>(type: "int", nullable: false),
                    number_of_assists = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scorers", x => x.scorer_id);
                    table.ForeignKey(
                        name: "FK_Scorers_Players",
                        column: x => x.player_id,
                        principalTable: "Players",
                        principalColumn: "player_id");
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    transfer_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    selling_user_id = table.Column<int>(type: "int", nullable: false),
                    buying_user_id = table.Column<int>(type: "int", nullable: false),
                    transfer_fee = table.Column<int>(type: "int", nullable: false),
                    transfer_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.transfer_id);
                    table.ForeignKey(
                        name: "FK_Transfers_Players",
                        column: x => x.player_id,
                        principalTable: "Players",
                        principalColumn: "player_id");
                    table.ForeignKey(
                        name: "FK_Transfers_Users_Buying",
                        column: x => x.buying_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Transfers_Users_Selling",
                        column: x => x.selling_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "TeamContracts",
                columns: table => new
                {
                    contract_id = table.Column<int>(type: "int", nullable: false),
                    squad_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    shirt_number = table.Column<int>(type: "int", nullable: false),
                    is_captain = table.Column<bool>(type: "bit", nullable: false),
                    is_first_team = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamContracts", x => x.contract_id);
                    table.ForeignKey(
                        name: "FK_Squad_players_Players",
                        column: x => x.player_id,
                        principalTable: "Players",
                        principalColumn: "player_id");
                    table.ForeignKey(
                        name: "FK_Squad_players_Squads1",
                        column: x => x.squad_id,
                        principalTable: "Squads",
                        principalColumn: "squad_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_player_id",
                table: "Awards",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_away_team_id",
                table: "Matches",
                column: "away_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_home_team_id",
                table: "Matches",
                column: "home_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_referee_id",
                table: "Matches",
                column: "referee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_stadium_id",
                table: "Matches",
                column: "stadium_id");

            migrationBuilder.CreateIndex(
                name: "IX_News_admin_id",
                table: "News",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_Players_league_id",
                table: "Players",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_Scorers_player_id",
                table: "Scorers",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_Squads_user_id",
                table: "Squads",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Standings_user_id",
                table: "Standings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamContracts_player_id",
                table: "TeamContracts",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamContracts_squad_id",
                table: "TeamContracts",
                column: "squad_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_buying_user_id",
                table: "Transfers",
                column: "buying_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_player_id",
                table: "Transfers",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_selling_user_id",
                table: "Transfers",
                column: "selling_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Formations");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Scorers");

            migrationBuilder.DropTable(
                name: "Standings");

            migrationBuilder.DropTable(
                name: "TeamContracts");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Referees");

            migrationBuilder.DropTable(
                name: "Stadiums");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Squads");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}

using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ClientApplication.Controllers
{

    //       Simulation Controller class
    public class SimulationController : Controller
    {
        internal class PlayersValue
        {
            public int Attack { get; set; }
            public int Control { get; set; }
            public int Defense { get; set; }

            public PlayersValue()
            {
                Attack = 0;
                Control = 0;
                Defense = 0;
            }
        }

        internal class StatusProcents
        {
            public byte Attackers { get; set; }
            public byte Midfielders { get; set; }
            public byte Defenders { get; set; }

            public StatusProcents()
            {
                Attackers = 0;
                Midfielders = 0;
                Defenders = 0;
            }
        }

        internal class TeamStatistics
        {
            public int TeamId { get; set; }

            public string TeamName { get; set; }

            public int Score;

            public int TeamAttack { get; set; }
            public int TeamControl { get; set; }
            public int TeamDefense { get; set; }
            public PlayersValue AttackersValue { get; set; }

            public PlayersValue MidfieldersValue { get; set; }

            public PlayersValue DefendersValue { get; set; }

            public TeamContract Striker { get; set; }
            public int StrikerValue { get; set; }
            public int GoalkeeperValue { get; set; }
            public StatusProcents AttackProcents { get; set; }

            public StatusProcents ControlProcents { get; set; }
            public StatusProcents DefenseProcents { get; set; }

            public TeamStatistics(int teamId)
            {
                TeamId = teamId;
                TeamName = string.Empty;
                AttackProcents = new StatusProcents();
                ControlProcents = new StatusProcents();
                DefenseProcents = new StatusProcents();
                AttackersValue = new PlayersValue();
                DefendersValue = new PlayersValue();
                MidfieldersValue = new PlayersValue();
                GoalkeeperValue = 0;
                TeamAttack = 0;
                TeamControl = 0;
                TeamDefense = 0;
            }
        }

        //      Each team has a specific random number generator. I felt like doing it this way.
        Random randomPlayer1 = new Random(Guid.NewGuid().GetHashCode());
        Random randomPlayer2 = new Random(Guid.NewGuid().GetHashCode());


        //      Events is a list of string values where I save values each time the simualation runs and an important event
        //  happens. As the number of simulation iterations is 18, each corresponding to 5 minutes of gameplay, I
        //  initialised the size of the List accordingly. This can be further increased or decreased, based on the
        //  frequency that we want our application to generate events.
        List<string> simulationEvents = new List<string>(18);
        int homeScore = 0;
        int awayScore = 0;

        //      _context is our current database context
        private readonly FootballDatabaseContext _context;

        //      The controller constructor
        public SimulationController(FootballDatabaseContext context)
        {
            _context = context;
        }

        //      The simulation method. It takes a match from the database and generates a simulation based
        //   on a mixture between random chance and the attributes of the players inside each team. Each
        //   simulation step will return a string witch marks an important event that took place during the time
        //   period. When the simulation is over we pass the string of events back to the match and when
        //   we look for a game we can see the full history of events that took place inside that period
        public void Simulation(Match match)
        {
            //      Get teams Id from the match model

            TeamStatistics homeTeam = new TeamStatistics(match.HomeTeamId);
            TeamStatistics awayTeam = new TeamStatistics(match.AwayTeamId);

            homeTeam.TeamName = _context.Squads.FirstOrDefault(m => m.SquadId == match.HomeTeamId).SquadName;
            awayTeam.TeamName = _context.Squads.FirstOrDefault(m => m.SquadId == match.AwayTeamId).SquadName;

            //      Get teams mentalities from match model
            byte mentalityHomeTeam = (byte)match.HomeMentality;
            byte mentalityAwayTeam = (byte)match.AwayMentality;

            GetPlayers(homeTeam);
            GetPlayers(awayTeam);

            GetProcents(homeTeam, mentalityHomeTeam);
            GetProcents(awayTeam, mentalityAwayTeam);

            GetTeamStatistics(homeTeam);
            GetTeamStatistics(awayTeam);

            // Simularea itself
            
            if (homeTeam.TeamId != 0)
            {
                for (int i = 1; i <= 18; i++)
                {
                    if (Possession(homeTeam, awayTeam))
                    {
                        chooseEvent(homeTeam, awayTeam);
                    }
                    else
                    {
                        chooseEvent(awayTeam, homeTeam);
                    }

                }
            }

            match.AwayGoals = awayScore;
            match.HomeGoals = homeScore;

            // Serialize the list of strings to JSON
            string EventsJson = JsonConvert.SerializeObject(simulationEvents.ToArray());
            match.Events = EventsJson;

            // Create or update the standings
            UpdateOrCreateStanding(match);

            // Save the changes to the database
            _context.SaveChanges();
        }


        private void UpdateOrCreateStanding(Match match)
        {
            foreach (Standing standing in _context.Standings)
            {
                if (match.HomeTeam.UserId == standing.UserId)
                {
                    SetStanding(standing, homeScore, awayScore);
                }
                else
                {
                    Standing newStanding = new Standing();
                    SetStanding(newStanding, homeScore, awayScore);
                }
                if (match.AwayTeam.UserId == standing.UserId)
                {
                    SetStanding(standing, awayScore, homeScore);
                }
                else
                {
                    Standing newStanding = new Standing();
                    SetStanding(newStanding, awayScore, homeScore);
                }
            }
        }

        private void SetStanding(Standing standing, int homeScore, int awayScore)
        {
            standing.MatchesPlayed++;
            standing.GoalsFor += homeScore;
            standing.GoalsAgainst += awayScore;
            if (homeScore > awayScore)
            {
                standing.MatchesWon++;
                standing.Trophies += 10;
                _context.Users.FirstOrDefault(m => m.UserId == standing.UserId).Coins += 100;

                // Save changes to db
                _context.SaveChanges();
            }
            else if (homeScore == awayScore)
            {
                standing.MatchesDrawn++;
                _context.Users.FirstOrDefault(m => m.UserId == standing.UserId).Coins += 50;

                // Save changes to db
                _context.SaveChanges();

            }
            else
            {
                standing.MatchesLost++;
                standing.Trophies -= 3;
                _context.Users.FirstOrDefault(m => m.UserId == standing.UserId).Coins += 10;
                
                // Save changes to db
                _context.SaveChanges();
            }
        }


            //      We make a database query to get all the players that make part of our hometeam and we save them
            //   inside a new local model called Team
            private List<Team> SelectPlayers(TeamStatistics team)
            {
                var TeamPlayersList = _context.Players
                        .Include(p => p.TeamContracts)
                        .ThenInclude(tc => tc.Squad)
                        .ThenInclude(s => s.User)
                        .Where(p => p.TeamContracts.Any(tc => tc.SquadId == team.TeamId))
                        .Select(p => new Team
                        {
                            Players = new List<Player> { p },
                            Contracts = p.TeamContracts.Where(tc => tc.SquadId == team.TeamId).ToList()
                        })
                        .ToList();

                return TeamPlayersList;
            }

            //      We make a database query that takes a lot of useful information about the players inside the team
            //   like their attacking, defending and control attributes, as well as their position in the team, which
            //   will indicate which role they will play.
            private void GetPlayers(TeamStatistics team)
            {
                var teamQuery = from player in _context.Players
                                join teamContract in _context.TeamContracts
                                on player.PlayerId equals teamContract.PlayerId
                                join squad in _context.Squads
                                on teamContract.SquadId equals squad.SquadId
                                where teamContract.SquadId == team.TeamId
                                select new
                                {
                                    PlayerId = player.PlayerId,
                                    SquadId = teamContract.SquadId,
                                    ContractId = teamContract.ContractId,
                                    Playerpos = player.Position,
                                    Overall = player.OverallRank,
                                    Attacking = player.Attacking,
                                    MidfieldControl = player.MidfieldControl,
                                    Defending = player.Defending,
                                    Position = teamContract.Position
                                };

                //      We then make a generic list using this data
                var Players = teamQuery.ToList();

                //      We make 4 extra lists that split the players into 4 big catogories: Attackers, Midfielders and Defenders, based on
                //   their Position.
                var teamGoalkeeper = Players.Where(r => r.Position == 1);
                var teamDefenders = Players.Where(r => r.Position == 2 || r.Position == 3 || r.Position == 4 || r.Position == 5).ToList();
                var teamMidfielders = Players.Where(r => r.Position == 6 || r.Position == 7 || r.Position == 8).ToList();
                var teamAttackers = Players.Where(r => r.Position == 9 || r.Position == 10 || r.Position == 11).ToList();
                var teamStriker = teamAttackers.Where(r => r.Overall == teamAttackers.Max(r => r.Overall));

                team.AttackersValue.Attack = (int)teamAttackers.Sum(x => x.Attacking);
                team.AttackersValue.Control = (int)teamAttackers.Sum(x => x.MidfieldControl);
                team.AttackersValue.Defense = (int)teamAttackers.Sum(x => x.Defending);
                team.MidfieldersValue.Attack = (int)teamMidfielders.Sum(x => x.Attacking);
                team.MidfieldersValue.Control = (int)teamMidfielders.Sum(x => x.MidfieldControl);
                team.MidfieldersValue.Defense = (int)teamMidfielders.Sum(x => x.Defending);
                team.DefendersValue.Attack = (int)teamDefenders.Sum(x => x.Attacking);
                team.DefendersValue.Control = (int)teamDefenders.Sum(x => x.MidfieldControl);
                team.DefendersValue.Defense = (int)teamDefenders.Sum(x => x.Defending);
                team.StrikerValue = teamStriker.Sum(x => x.Playerpos.Contains("ST") || x.Playerpos.Contains("CF") || x.Playerpos.Contains("LW") || x.Playerpos.Contains("RW") ? x.Overall : x.Overall / 10);
                team.GoalkeeperValue = teamGoalkeeper.Sum(x => x.Playerpos.Contains("GK") ? x.Overall : x.Overall / 10);

                //Select squadId and playId from the teamContract
                /*
                foreach (var id in teamStriker.Select(y => y.PlayerId))
                {
                   team.Striker.PlayerId = id;
                }
                foreach (var id in teamStriker.Select(y => y.SquadId))
                {
                    team.Striker.SquadId = id;
                }*/



            }

            //      Moving on to the simulation logic, we will now establish the statistics of each team
            //   which is needed for the simulation process. For that, each team's value will correspond
            //   to the attacking, midfield control and defending values of each of its players. Each player
            //   is assigned a procent of its total value to contribute towards the team's overall rating. These
            //   procents change depending on the mentality the team uses. The reason I took an extra step to
            //   introduce this mechanic is because I want to make the simulation as balanced as possible and
            //   build into the intuition of having players play their respective positions and punish the teams
            //   that don't build their team correctly.

            private void GetProcents(TeamStatistics team, byte mentality)
            {
                //      All procents are assigned using a byte data type to save space. The control procents are not changed
                //   regardless of the mentality as for our application's current state there is no need.
                //   However, in future releases this may be a subject to change.

                const byte ControlProcentOfTheAttackers = 20;       // 20% of the attacker's control will be used for the team's total control
                const byte ControlProcentOfTheMidfielders = 60;     // 60% of the midfielder's control will be used for the team's total control
                const byte ControlProcentOfTheDefenders = 20;       // 20% of the defender's control will be used for the team's total control

                //      I am saving the values inside an array

                byte attackProcentOfTheAttackers;
                byte attackProcentOfTheMidfielders;
                byte attackProcentOfTheDefenders;

                byte defenseProcentOfTheAttackers;
                byte defenseProcentOfTheMidfielders;
                byte defenseProcentOfTheDefenders;

                if (mentality == 1)   // attacking mentality
                {
                    attackProcentOfTheAttackers = 80;
                    attackProcentOfTheMidfielders = 15;
                    attackProcentOfTheDefenders = 5;

                    defenseProcentOfTheAttackers = 40;
                    defenseProcentOfTheMidfielders = 30;
                    defenseProcentOfTheDefenders = 30;
                }
                else if (mentality == 2)   // defensive mentality
                {
                    attackProcentOfTheAttackers = 30;
                    attackProcentOfTheMidfielders = 30;
                    attackProcentOfTheDefenders = 30;

                    defenseProcentOfTheAttackers = 5;
                    defenseProcentOfTheMidfielders = 15;
                    defenseProcentOfTheDefenders = 70;
                }
                else
                {
                    // Handle other cases or provide default values
                    attackProcentOfTheAttackers = 60;
                    attackProcentOfTheMidfielders = 30;
                    attackProcentOfTheDefenders = 20;

                    defenseProcentOfTheAttackers = 20;
                    defenseProcentOfTheMidfielders = 30;
                    defenseProcentOfTheDefenders = 50;
                }

                team.AttackProcents.Attackers = attackProcentOfTheAttackers;
                team.AttackProcents.Midfielders = attackProcentOfTheMidfielders;
                team.AttackProcents.Defenders = attackProcentOfTheDefenders;
                team.ControlProcents.Attackers = ControlProcentOfTheAttackers;
                team.ControlProcents.Midfielders = ControlProcentOfTheMidfielders;
                team.ControlProcents.Defenders = ControlProcentOfTheDefenders;
                team.DefenseProcents.Attackers = defenseProcentOfTheAttackers;
                team.DefenseProcents.Midfielders = defenseProcentOfTheMidfielders;
                team.DefenseProcents.Defenders = defenseProcentOfTheDefenders;

            }

            private void GetTeamStatistics(TeamStatistics team)
            {
                team.TeamAttack = (team.AttackersValue.Attack * team.AttackProcents.Attackers +
                    team.MidfieldersValue.Attack * team.AttackProcents.Midfielders +
                    team.DefendersValue.Attack * team.AttackProcents.Defenders) / 100;

                team.TeamControl = (team.AttackersValue.Control * team.ControlProcents.Attackers +
                    team.MidfieldersValue.Control * team.ControlProcents.Midfielders +
                    team.DefendersValue.Control * team.ControlProcents.Defenders) / 100;

                team.TeamDefense = (team.AttackersValue.Defense * team.DefenseProcents.Attackers +
                    team.MidfieldersValue.Defense * team.DefenseProcents.Midfielders +
                    team.DefendersValue.Defense * team.DefenseProcents.Defenders) / 100;
            }


            private bool Possession(TeamStatistics team1, TeamStatistics team2)
            {
                if (team1 == null || team2 == null) return false;
                else if (team1.TeamControl + randomPlayer1.Next(1, 150) >
                            team2.TeamControl + randomPlayer2.Next(1, 150))
                {
                    return true;
                }
                else
                    return false;
            }

            private int Goal(TeamStatistics team1, TeamStatistics team2)
            {
                int team1Luck = randomPlayer1.Next(10, 150);
                int team2Luck = randomPlayer2.Next(10, 650);
                return (team1.TeamAttack + team1Luck) -
                     (team2.TeamDefense + team2Luck + team2.GoalkeeperValue);
            }

            private int Penalty(TeamStatistics team1, TeamStatistics team2)
            {
                int team1Luck = randomPlayer1.Next(1, 141);
                int team2Luck = randomPlayer2.Next(1, 101);
                return (team1.StrikerValue + team1Luck) -
                     (team2.GoalkeeperValue + team2Luck);
            }

            private int FreeKick(TeamStatistics team1, TeamStatistics team2)
            {
                int team1Luck = randomPlayer1.Next(1, 51);
                int team2Luck = randomPlayer2.Next(1, 101);
                return (team1.StrikerValue + team1Luck) -
                     (team2.GoalkeeperValue + team2Luck);
            }
            private int CounterAttack()
            {
                return (randomPlayer2.Next(1, 100) - 90);
            }

            private string RandomEvent(TeamStatistics homeTeam)
            {
                int randomEvent = randomPlayer1.Next(1, 8);
                switch (randomEvent)
                {
                    case 1:
                        return homeTeam.TeamName + " has possession of the ball!";
                    case 2:
                        return "Dangerous attack for " + homeTeam.TeamName + "!";
                    case 3:
                        return homeTeam.TeamName + " controls the ball!";
                    case 4:
                        return "Crossing opportunity for " + homeTeam.TeamName + "!";
                    case 5:
                        return "The referee catches" + GoalScorer(SelectPlayers(homeTeam)) + "in offsite";
                    case 6:
                        return "Corner kick for " + homeTeam.TeamName + "!";
                    case 7:
                        return GoalScorer(SelectPlayers(homeTeam)) + " has an amazing tackle!";
                    default:
                        return homeTeam.TeamName;
                }
            }

            private void chooseEvent(TeamStatistics homeTeam, TeamStatistics awayTeam)
            {
                int simulationValue;
                int scoreInitial = homeTeam.Score;
                int currentEvent = randomPlayer1.Next(1, 20);
                if (currentEvent <= 12)
                {
                    simulationValue = Goal(homeTeam, awayTeam);
                    if (simulationValue >= 0)
                    {
                        simulationEvents.Add(GoalScorer(SelectPlayers(homeTeam)) + " scored a goal for "
                                    + homeTeam.TeamName + "!");
                        homeTeam.Score++;
                    } else if (simulationValue >= -20)
                    {
                        simulationEvents.Add(GoalScorer(SelectPlayers(homeTeam)) + " barely missed the Goal!");
                    }
                    else
                        simulationEvents.Add(RandomEvent(homeTeam));
                }
                else if (currentEvent <= 18)
                {
                    simulationValue = FreeKick(homeTeam, awayTeam);
                    if (simulationValue >= 0)
                    {
                        simulationEvents.Add(GoalScorer(SelectPlayers(homeTeam)) + " scores exemplary from a free kick for "
                                     + homeTeam.TeamName + "!");
                        homeTeam.Score++;
                    } else if (simulationValue >= -20)
                    {
                        simulationEvents.Add(GoalScorer(SelectPlayers(homeTeam)) + "'s kick is miraculously saved by the goalkeeper!");
                    }
                    else
                        simulationEvents.Add(RandomEvent(homeTeam));
                }
                else
                {
                    simulationValue = Penalty(homeTeam, awayTeam);
                    if (simulationValue >= 0)
                    {
                        simulationEvents.Add("Penalty scored by " + GoalScorer(SelectPlayers(homeTeam)) + " for "
                                     + homeTeam.TeamName + "!");
                        homeTeam.Score++;
                    } else if (simulationValue >= -20)
                    {
                        simulationEvents.Add(GoalScorer(SelectPlayers(homeTeam)) + "'s kick is off the crossbar!");
                    }
                    simulationEvents.Add(RandomEvent(homeTeam));
                }
                if (scoreInitial == homeTeam.Score)
                {
                    simulationValue = CounterAttack();
                    if (simulationValue >= 0)
                    {
                        simulationEvents.Add("Sensational goal scored by " + GoalScorer(SelectPlayers(homeTeam)) + " for "
                                     + awayTeam.TeamName + " on a counter attack!");
                        awayTeam.Score++;
                    }

                }
            }

            //      Function to determine goalscorer
            private string GoalScorer(List<Team> team)
            {
                //      The process will select a random number between 1 and 10 that will associate with the scorer
                //   1-6 means that the scorer is an attacker; 7-9 means the scorer is a midfielder
                //   and if the random score is exactly 10, then the scorer is a defender
                Random goalScorerRandom = new Random(Guid.NewGuid().GetHashCode());

                int goalScorerCurrentRandom = goalScorerRandom.Next(0, 11);


                //      Condition for an attacker to score a goal
                if (goalScorerCurrentRandom < 7)
                {
                    //     ! DISCLAIMER ! this implementation is very unethical and we would like to 
                    //   change this as soon as possible, but for that is required an entire revamp
                    //   of the database, so bear with us for now.

                    //     This random gives even chance for any of the 3 attackers to score a goal
                    //   followed by a query through the models to extract the player's firstname and lastname
                    goalScorerCurrentRandom = goalScorerRandom.Next(9, 12);

                    //     Going through each team in the list of teams
                    foreach (Team echipa in team)
                    {
                        //      Going through each player in the current team
                        foreach (Player player in echipa.Players)
                        {
                            foreach (TeamContract contract in echipa.Contracts)
                            {
                                //      Checking to see if the current player has a contract with the current
                                //      team to get the player at the position our random number has generated
                                if (contract.Position == goalScorerCurrentRandom)
                                {
                                    // Increment the player's goals in Scorers table
                                    //player.Goals ++;
                                    return player.FirstName + " " + player.LastName;
                                }
                            }
                        }
                    }
                }

                //      Condition for a midfielder to score a goal
                else if (goalScorerCurrentRandom < 10)
                {
                    goalScorerCurrentRandom = goalScorerRandom.Next(6, 9);
                    foreach (Team echipa in team)
                    {
                        foreach (Player player in echipa.Players)
                        {
                            foreach (TeamContract contract in echipa.Contracts)
                            {
                                // Get player at position random from contracts
                                if (contract.Position == goalScorerCurrentRandom)
                                {
                                    return player.FirstName + " " + player.LastName;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //      Condition for a midfielder to score a goal
                    goalScorerCurrentRandom = goalScorerRandom.Next(2, 6);
                    // Get the player on position random
                    foreach (Team echipa in team)
                    {
                        foreach (Player player in echipa.Players)
                        {
                            foreach (TeamContract contract in echipa.Contracts)
                            {
                                // Get player at position random from contracts
                                if (contract.Position == goalScorerCurrentRandom)
                                {
                                    return player.FirstName + " " + player.LastName;
                                }
                            }
                        }
                    }
                }

                //      In case we something goes wrong, we return an Error message
                return "Error";
            }

            /*
            private string GoalScorer(Player striker)
            {
                var player = _context.Players.FirstOrDefault(p => p.PlayerId == striker.PlayerId);
                var teamContract = _context.TeamContracts.FirstOrDefault(tc => tc.PlayerId == player.PlayerId && tc.SquadId == striker.SquadId);

                foreach (Player player in Players)
                {
                    foreach (TeamContract contract in echipa.Contracts)
                    {
                        //      Checking to see if the current player has a contract with the current
                        //      team to get the player at the position our random number has generated
                        if (contract.Position == goalScorerCurrentRandom)
                        {
                            return player.FirstName + " " + player.LastName;
                        }
                    }
                }

            }
            */

    }
}

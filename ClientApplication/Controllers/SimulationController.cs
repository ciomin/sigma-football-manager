using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Versioning;
using System.Drawing.Text;

namespace ClientApplication.Controllers
{
    public class SimulationController : Controller
    {
        Random rnd1 = new Random(Guid.NewGuid().GetHashCode());
        Random rnd2 = new Random(Guid.NewGuid().GetHashCode());
         
        List<string> events = new List<string>();

        private readonly FootballDatabaseContext _context;

        public SimulationController(FootballDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Function to determine goalscorer
        private string GoalScorer(List<Team> team)
        {
            // Random number to deetrmine goalscorer
            Random rnd3 = new Random(Guid.NewGuid().GetHashCode());

            // Get random number between 0 and 10
            int random = rnd3.Next(0, 11);

            if (random < 7)
            {
                // Attacker scores
                random = rnd3.Next(9, 12);
                // Get the player on position random
                foreach (Team echipa in team)
                {
                    foreach (Player player in echipa.Players)
                    {
                        foreach (TeamContract contract in echipa.Contracts)
                        {
                            // Get player at position random from contracts
                            if (contract.Position == random)
                            {
                                return player.FirstName + " " + player.LastName;
                            }
                        }
                    }
                }
            }
            else if (random < 10)
            {
                // Midfielder scores
                random = rnd3.Next(6, 9);
                // Get the player on position random
                foreach (Team echipa in team)
                {
                    foreach (Player player in echipa.Players)
                    {
                        foreach (TeamContract contract in echipa.Contracts)
                        {
                            // Get player at position random from contracts
                            if (contract.Position == random)
                            {
                                return player.FirstName + " " + player.LastName;
                            }
                        }
                    }
                }
            }
            else
            {
                // Defender scores
                random = rnd3.Next(2, 6);
                // Get the player on position random
                foreach (Team echipa in team)
                {
                    foreach (Player player in echipa.Players)
                    {
                        foreach (TeamContract contract in echipa.Contracts)
                        {
                            // Get player at position random from contracts
                            if (contract.Position == random)
                            {
                                return player.FirstName + " " + player.LastName;
                            }
                        }
                    }
                }
            }
            return "Error";
        }

        public void Simulation(Match match)
        {
            // Get teams id from match
            int team1_id = match.HomeTeamId;
            int team2_id = match.AwayTeamId;

            // Get mentalities from match 
            int mentality1 = (int)match.HomeMentality;
            int mentality2 = (int)match.AwayMentality;

            // Get list of players from team 1
            var players1_list = _context.Players
                    .Include(p => p.TeamContracts)
                    .ThenInclude(tc => tc.Squad)
                    .ThenInclude(s => s.User)
                    .Where(p => p.TeamContracts.Any(tc => tc.SquadId == match.HomeTeamId))
                    .Select(p => new Team
                    {
                        Players = new List<Player> { p },
                        Contracts = p.TeamContracts.Where(tc => tc.SquadId == match.HomeTeamId).ToList()
                    })
                    .ToList();

            // Get list of players from team 2
            var players2_list = _context.Players
                    .Include(p => p.TeamContracts)
                    .ThenInclude(tc => tc.Squad)
                    .ThenInclude(s => s.User)
                    .Where(p => p.TeamContracts.Any(tc => tc.SquadId == match.AwayTeamId))
                    .Select(p => new Team
                    {
                        Players = new List<Player> { p },
                        Contracts = p.TeamContracts.Where(tc => tc.SquadId == match.AwayTeamId).ToList()
                    })
                    .ToList();

            var query1 = from player in _context.Players
                         join teamContract in _context.TeamContracts
                         on player.PlayerId equals teamContract.PlayerId
                         join squad in _context.Squads
                         on teamContract.SquadId equals squad.SquadId
                         where teamContract.SquadId == team1_id
                         select new
                         {
                             PlayerId = player.PlayerId,
                             FirstName = player.FirstName,
                             LastName = player.LastName,
                             Playerpos = player.Position,
                             ContractId = teamContract.ContractId,
                             SquadId = squad.SquadId,
                             SquadName = squad.SquadName,
                             Overall = player.OverallRank,
                             Attacking = player.Attacking,
                             MidfieldControl = player.MidfieldControl,
                             Defending = player.Defending,
                             Position = teamContract.Position
                         };
            var players1 = query1.ToList();

            var goalkeeper_team_1 = players1.Where(r => r.Position == 1);
            var defenders_team_1 = players1.Where(r => r.Position == 2 || r.Position == 3 || r.Position == 4 || r.Position == 5).ToList();
            var midfielders_team_1 = players1.Where(r => r.Position == 6 || r.Position == 7 || r.Position == 8).ToList();
            var attackers_team_1 = players1.Where(r => r.Position == 9 || r.Position == 10 || r.Position == 11).ToList();

            var query2 = from player in _context.Players
                         join teamContract in _context.TeamContracts
                         on player.PlayerId equals teamContract.PlayerId
                         join squad in _context.Squads
                         on teamContract.SquadId equals squad.SquadId
                         where teamContract.SquadId == team2_id
                         select new
                         {
                             PlayerId = player.PlayerId,
                             FirstName = player.FirstName,
                             LastName = player.LastName,
                             Playerpos = player.Position,
                             ContractId = teamContract.ContractId,
                             SquadId = squad.SquadId,
                             SquadName = squad.SquadName,
                             Overall = player.OverallRank,
                             Attacking = player.Attacking,
                             MidfieldControl = player.MidfieldControl,
                             Defending = player.Defending,
                             Position = teamContract.Position
                         };
            var players2 = query2.ToList();

            var goalkeeper_team_2 = players2.Where(r => r.Position == 1);
            var defenders_team_2 = players2.Where(r => r.Position == 2 || r.Position == 3 || r.Position == 4 || r.Position == 5).ToList();
            var midfielders_team_2 = players2.Where(r => r.Position == 6 || r.Position == 7 || r.Position == 8).ToList();
            var attackers_team_2 = players2.Where(r => r.Position == 9 || r.Position == 10 || r.Position == 11).ToList();

            int[] cnt_procents = new int[3] { 20, 60, 20 };

            int[] att_procents1 = new int[3];
            int[] def_procents1 = new int[3];
            
            //--------------------Team 1--------------------
            if (mentality1 == 0)   // balanced mentality
            {
                att_procents1 = new int[] { 50, 30, 20 };
                def_procents1 = new int[] { 20, 40, 40 };
            }
            if (mentality1 == 1)   // attacking mentality
            {
                att_procents1 = new int[] { 80, 15, 5 };
                def_procents1 = new int[] { 40, 30, 30 };
            }
            if (mentality1 == 2)   // deffensive mentality
            {
                att_procents1 = new int[] { 20, 50, 25 };
                def_procents1 = new int[] { 15, 20, 60 };
            }

            int[] att_procents2 = new int[3];
            int[] def_procents2 = new int[3];

            //-------------------Team 2---------------------
            if (mentality2 == 0)   // balanced mentality
            {
                att_procents2 = new int[] { 50, 30, 20 };
                def_procents2 = new int[] { 20, 40, 40 };
            }
            if (mentality2 == 1)   // attacking mentality
            {
                att_procents2 = new int[] { 80, 15, 5 };
                def_procents2 = new int[] { 40, 30, 30 };
            }
            if (mentality2 == 2)   // deffensive mentality
            {
                att_procents2 = new int[] { 20, 50, 25 };
                def_procents2 = new int[] { 15, 20, 60 };
            }

            //--------------------Logic----------------------
            int[] team_1_attack = new int[3] { (int)(attackers_team_1.Sum(x => x.Attacking) * att_procents1[0] / 100),
                (int)(midfielders_team_1.Sum(x => x.Attacking) * att_procents1[1] / 100),
                (int)(defenders_team_1.Sum(x => x.Attacking) * att_procents1[2] / 100)};

            int[] team_2_attack = new int[3] { (int)(attackers_team_2.Sum(x => x.Attacking) * att_procents2[0] / 100),
                (int)(midfielders_team_2.Sum(x => x.Attacking) * att_procents2[1] / 100),
                (int)(defenders_team_2.Sum(x => x.Attacking) * att_procents2[2] / 100)};

            int[] team_1_control = new int[3] {(int)(attackers_team_1.Sum(x => x.MidfieldControl) * cnt_procents[0] / 100),
                (int)(midfielders_team_1.Sum(x => x.MidfieldControl) * cnt_procents[1] / 100),
                (int)(defenders_team_1.Sum(x => x.MidfieldControl) * cnt_procents[2] / 100) };

            int[] team_2_control = new int[3] {(int)(attackers_team_2.Sum(x => x.MidfieldControl) * cnt_procents[0] / 100),
                (int)(midfielders_team_2.Sum(x => x.MidfieldControl) * cnt_procents[1] / 100),
                (int)(defenders_team_2.Sum(x => x.MidfieldControl) * cnt_procents[2] / 100) };

            int[] team_1_defense = new int[3] {(int)(attackers_team_1.Sum(x => x.Defending) * def_procents1[0] / 100),
                (int)(midfielders_team_1.Sum(x => x.Defending) * def_procents1[1] / 100),
                (int)(defenders_team_1.Sum(x => x.Defending) * def_procents1[2] / 100) };

            int[] team_2_defense = new int[3] {(int)(attackers_team_2.Sum(x => x.Defending) * def_procents2[0] / 100),
                (int)(midfielders_team_2.Sum(x => x.Defending) * def_procents2[1] / 100),
                (int)(defenders_team_2.Sum(x => x.Defending) * def_procents2[2] / 100) };

            int[] team_1 = new int[3] { team_1_attack.Sum(), team_1_control.Sum(), team_1_defense.Sum() };
            int[] team_2 = new int[3] { team_2_attack.Sum(), team_2_control.Sum(), team_2_defense.Sum() };


            List<string> teams = new List<string>();
            List<string> attributes = new List<string>();

            foreach (var players in players1)
            {
                if (!teams.Contains(players.SquadName))
                    teams.Add(players.SquadName);
            }
            foreach (var players in players2)
            {
                if (!teams.Contains(players.SquadName))
                    teams.Add(players.SquadName);
            }
            foreach (int attribute in team_1)
            {
                attributes.Add(attribute.ToString());
            }
            foreach (int attribute in team_2)
            {
                attributes.Add(attribute.ToString());
            }

            // Simularea itself
            int score_team_1 = 0;
            int score_team_2 = 0;
            if (attackers_team_1.Count() != 0)
            {
                for (int i = 1; i <= 18; i++)
                {
                    string result;
                    if (team_1[1] + rnd1.Next(1, 150) > team_2[1] + rnd2.Next(1, 150))
                    {
                        if (team_1[0] + rnd1.Next(10, 200) > team_2[2] + rnd2.Next(10, 600) + goalkeeper_team_2.Sum(x => x.Playerpos == "GK" ? x.Overall : x.Overall / 10))
                        {

                            result = GoalScorer(players1_list) + " scored a goal for "
                                + attackers_team_1.Last().SquadName.ToString() + "!";
                            score_team_1++;
                            events.Add(result);
                        }
                        else
                        {
                            // Chance of penalty
                            if (rnd1.Next(1, 100) > 98)
                            {
                                result = "Penalty scored by " + GoalScorer(players1_list) + " for "
                                 + attackers_team_1.Last().SquadName.ToString() + "!";
                                score_team_1++;
                                events.Add(result);
                            }
                            else if (rnd2.Next(1, 100) > 97)
                            {
                                // Chance of counter attack goal 
                                result = "Sensational goal scored by " + GoalScorer(players2_list) + " for "
                                + attackers_team_2.Last().SquadName.ToString() + " on a counter attack!";
                                score_team_2++;
                                events.Add(result);
                            }
                            else
                            {
                                result = attackers_team_1.First().SquadName.ToString() + " has possession of the ball!";
                                events.Add(result);
                            }
                        }
                    }
                    else
                    {
                        if (team_2[0] + rnd2.Next(1, 50) > team_1[2] + goalkeeper_team_1.Sum(x => x.Playerpos == "GK" ? x.Overall : x.Overall / 10))
                        {
                            result = GoalScorer(players2_list) + " scored goal for "
                                + attackers_team_2.Last().SquadName.ToString() + "!";
                            score_team_2++;
                            events.Add(result);
                        }
                        else
                        {
                            // Chance of penalty
                            if (rnd2.Next(1, 100) > 98)
                            {
                                result = "Penalty scored by " + GoalScorer(players2_list) + " for "
                                 + attackers_team_2.Last().SquadName.ToString() + "!";
                                score_team_2++;
                                events.Add(result);
                            }
                            else if (rnd1.Next(1, 100) > 97)
                            {
                                // Chance of counter attack goal
                                result = "Sensational goal scored by " + GoalScorer(players1_list) + " for "
                                 + attackers_team_1.Last().SquadName.ToString() + " on a counter attack!";
                                score_team_1++;
                                events.Add(result);
                            }
                            else
                            {
                                result = attackers_team_2.First().SquadName.ToString() + " has possession of the ball!";
                                events.Add(result);
                            }
                        }
                    }

                }
            }

            // Adding the score to the match
            match.AwayGoals = score_team_2;
            match.HomeGoals = score_team_1;

            // Adding the events to the match

            // Serialize the list of strings to JSON
            string EventsJson = JsonConvert.SerializeObject(events.ToArray());
            match.Events = EventsJson;

            // Save the changes to the database
            _context.SaveChanges();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientApplication.Models;

using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

namespace ClientApplication.Controllers
{
    public class GameController : Controller
    {

        private readonly FootballDatabaseContext _context;
        private readonly Dictionary<int, string> _userConnections = new Dictionary<int, string>();

        public GameController(FootballDatabaseContext context)
        {
            _context = context;
        }

        // This is a list of all the users that are waiting for a match
        private static readonly List<int> _waitingRequests = new List<int>();
        private static readonly List<int> _waitingMentalities = new List<int>();
        
        // This is a flag that will be used to notify the user that a match has been found
        private static readonly List<MatchmakingWait> waitFlags = new List<MatchmakingWait>();

        [HttpPost]
        public async Task<IActionResult> JoinMatchmakingAsync(int mentality)
        {
            int userID = 0;
            // Get the id of the user that is currently logged in
            var username = HomeController.GetUserName(HttpContext);

            if (username == null)
                return RedirectToAction("Login", "Access");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            userID = user.UserId;

            // If user is already in the waiting list, redirect him to the matchmaking page
            if (_waitingRequests.Contains(userID))
                return RedirectToAction("GameRoom");
   
            lock (_waitingRequests)
            {
                // Add the user to the waiting list
                _waitingMentalities.Add(mentality);
                _waitingRequests.Add(userID);

                if (_waitingRequests.Count >= 2)
                {
                    // The first two users in the list will be matched
                    int user1 = _waitingRequests[0];
                    int user2 = _waitingRequests[1];
                    int mentality1 = _waitingMentalities[0];
                    int mentality2 = _waitingMentalities[1];

                    // Remove the users from the list
                    _waitingRequests.RemoveAt(0);
                    _waitingRequests.RemoveAt(0);
                    _waitingMentalities.RemoveAt(0);
                    _waitingMentalities.RemoveAt(0);

                    // Create a new match for them
                    int matchId = CreateMatch(user1, user2, mentality1, mentality2);

                    // Notify user1 that a match has been found using the existing flag with his id
                    MatchmakingWait waitFlag1 = waitFlags.FirstOrDefault(w => w.UserId == user1);
                    waitFlag1.MatchId = matchId;
                    waitFlag1.MatchFound = true;

                    // Redirect the users to the game session
                    return RedirectToAction("GameSession", new { matchId });
                }
            }

            // Add the user to he list of flags that will be used to notify him that a match has been found
            MatchmakingWait waitFlag = new MatchmakingWait();
            waitFlag.UserId = userID;
            waitFlag.MatchFound = false;
            waitFlags.Add(waitFlag);

            return RedirectToAction("GameRoom", "Game");
        }

        
        private int CreateMatch(int user1, int user2, int mentality1, int mentality2)
        {
            // Create a new match
            // Generate a new id for the match using Match model
            Match match = new Match();

            // If the match table is empty, the id will be 1, otherwise it will be the max id + 1
            if (_context.Matches.Count() == 0)
                match.MatchId = 1;
            else
                match.MatchId = _context.Matches.Max(m => m.MatchId) + 1;

            // Add the two users to the match
            match.HomeTeamId = user1;
            match.AwayTeamId = user2;

            // Add a random stadium to the match
            Random rnd = new Random();
            int stadiumId = rnd.Next(1, _context.Stadiums.Count() + 1);
            match.StadiumId = stadiumId;

            // Add a random referee to the match    
            int refereeId = rnd.Next(1, _context.Referees.Count() + 1);
            match.RefereeId = refereeId;

            match.HomeMentality = mentality1;
            match.AwayMentality = mentality2;

            // Add the match to the database
            _context.Matches.Add(match);
            _context.SaveChanges();

            // Simulate the match
            SimulationController simulation = new SimulationController(_context);
            simulation.Simulation(match);

            return match.MatchId;
        }


        // The actual simulation, both users will be redirected to this page
        [HttpGet]
        public async Task<IActionResult> GameSession(string matchId)
        {
            int match_Id = Int32.Parse(matchId);
            List<string> events = new List<string>();

            // Get the id's of the two users that are playing the match
            Match match = _context.Matches.FirstOrDefault(m => m.MatchId == match_Id);
            int user1 = match.HomeTeamId;
            int user2 = match.AwayTeamId;

            // Bind the two users to the match
            match.AwayTeam = _context.Users.FirstOrDefault(u => u.UserId == user2);
            match.HomeTeam = _context.Users.FirstOrDefault(u => u.UserId == user1);

            // Bind the stadium to the match
            match.Stadium = _context.Stadiums.FirstOrDefault(s => s.StadiumId == match.StadiumId);

            // Bind the referee to the match
            match.Referee = _context.Referees.FirstOrDefault(r => r.RefereeId == match.RefereeId);

            // Retrieve serialized events from the database
            string serializedEvents = match.Events;
            events = JsonConvert.DeserializeObject<List<string>>(serializedEvents);

            ViewBag.Events = events;

            // For shared view
            ViewBag.wallet = getMoney();
            ViewBag.username = HomeController.GetUserName(HttpContext);

            return View(match);
        }


        // Simple menu for the user to select the mentality of his team and join a matchmaking queue
        [HttpGet]
        public IActionResult GameMenu()
        {
            // Get the id of the user that is currently logged in
            var username = HomeController.GetUserName(HttpContext);

            if (username == null)
                return RedirectToAction("Login", "Access");
            
            ViewBag.username = username;

            // Add nr of coins to the wallet
            ViewBag.wallet = getMoney();
            return View();
        }

        // Waiting page for the user
        [HttpGet]
        public IActionResult GameRoom()
        {
            // Get the flag variable for the user   
            var username = HomeController.GetUserName(HttpContext);
            ViewBag.userame = username;

            User user = _context.Users.FirstOrDefault(u => u.Username == username);
            int userID = user.UserId;

            MatchmakingWait waitFlag = waitFlags.FirstOrDefault(w => w.UserId == userID);

            // Add nr of coins to the wallet
            ViewBag.wallet = getMoney();

            return View(user);
        }


        // Returns a flag for the user waiting in GameRoom
        [HttpGet]
        public IActionResult CheckMatchStatus(int userId)
        {
            // Get the flag variable for the user
            MatchmakingWait waitFlag = waitFlags.FirstOrDefault(w => w.UserId == userId);
            
            if (waitFlag != null)
            {
                // Check the flag variable to determine if the match has been found
                bool matchFound = waitFlag.MatchFound;
                // return the flag variable and the match id
                return Json(new { matchFound, matchId = waitFlag.MatchId });
            }
            else
            {
                return Json(new { matchFound = false });
            }
        }

        // Deletes the flag variable for the user
        [HttpGet]
        public IActionResult DeleteWaitFlag(int userId)
        {
            MatchmakingWait waitFlag = waitFlags.FirstOrDefault(w => w.UserId == userId);
            
            // Delete the flag variable
            waitFlags.Remove(waitFlag);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult AfterGame()
        {
            // Add nr of coins to the wallet
            ViewBag.wallet = getMoney();

            // Get the username if the user is logged in
            var userName = HomeController.GetUserName(HttpContext);
            if(userName == null)
            {
                return RedirectToAction("Login", "Access");
            }

            // Add the username to the viewbag
            ViewBag.username = userName;

            // Get the user's id
            var user = _context.Users.FirstOrDefault(u => u.Username == userName);
            if (user != null)
            {
                var match = _context.Matches
                        .Where(m => m.HomeTeamId == user.UserId || m.AwayTeamId == user.UserId)
                        .OrderBy(m => m.MatchId) // Add an OrderBy operation
                        .LastOrDefault();
                if (match != null)
                {
                    // If the user was away team, swap the home and away team
                    if (match.AwayTeamId == user.UserId)
                    {
                        var temp = match.HomeTeamId;
                        // Swap id's
                        match.HomeTeamId = match.AwayTeamId;
                        match.AwayTeamId = temp;

                        // Swap Scores
                        var tempScore = match.HomeGoals;
                        match.HomeGoals = match.AwayGoals;
                        match.AwayGoals = tempScore;
                    }

                    // Return a view that displays the results of the match
                    return View(match);
                }
            }
            return View();
        }

        public int getMoney()
        {
            // Check is user is logged in
            if (HomeController.GetUserName(HttpContext) == null)
            {
                return 0;
            }
            else
            {
                // Get the user's coins
                var userName = HomeController.GetUserName(HttpContext);
                var user = _context.Users.FirstOrDefault(u => u.Username == userName);
                return user.Coins;
            }
        }
    }

}

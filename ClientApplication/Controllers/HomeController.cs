using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FootballManager_v0._1.Models;

namespace ClientApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly FootballDatabaseContext _context;

		public HomeController(ILogger<HomeController> logger, FootballDatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Store()
        {
            ViewBag.username = HomeController.GetUserName(HttpContext);
            ViewBag.cheapPlayers = getCheapPlayers();
			ViewBag.regularPlayers = getRegularPlayers();
			ViewBag.expensivePlayers = getExpensivePlayers();
            ViewBag.wallet = getMoney();
			return View();
        }

        public int getMoney()
        {
			var userName = GetUserName(HttpContext);
			var user = _context.Users.FirstOrDefault(u => u.Username == userName);

			return user.Coins;
        }

        [HttpPost]
        public IActionResult updateMoney(int value)
        {
			var userName = GetUserName(HttpContext);
			var user = _context.Users.FirstOrDefault(u => u.Username == userName);

            user.Coins = value;
            _context.SaveChanges();

			return Json(new { result = "success" });
		}

        public List<Player> getCheapPlayers()
        {
            List<Player> list = new List<Player>();
            List<Player> plist = _context.Players.ToList();

            foreach (Player p in plist)
            {
                if(p.OverallRank <= 75)
                {
                    list.Add(p);
                }
            }
            return list;
        }

		public List<Player> getRegularPlayers()
		{
			List<Player> list = new List<Player>();
			List<Player> plist = _context.Players.ToList();

			foreach (Player p in plist)
			{
				if (p.OverallRank > 75 && p.OverallRank < 80)
				{
					list.Add(p);
				}
			}
			return list;
		}

		public List<Player> getExpensivePlayers()
		{
			List<Player> list = new List<Player>();
			List<Player> plist = _context.Players.ToList();

			foreach (Player p in plist)
			{
				if (p.OverallRank >= 80)
				{
					list.Add(p);
				}
			}
			return list;
		}

        [HttpPost]
        public IActionResult addPlayerTeam(int[] playerIds)
        {
			var userName = GetUserName(HttpContext);
			var user = _context.Users.FirstOrDefault(u => u.Username == userName);
            var squad = _context.Squads.FirstOrDefault(s => s.UserId == user.UserId);
            var contract = new TeamContract();

            int cNumber = _context.TeamContracts.Count() + 1;

            foreach(int id in playerIds)
            {
                contract.ContractId = cNumber;
                contract.SquadId = squad.SquadId;
                contract.PlayerId = id;
                contract.ShirtNumber = null;
                contract.IsCaptain = null;
                contract.Position = 19;
                cNumber++;
				_context.TeamContracts.Add(contract);
				_context.SaveChanges();

			}

			return Json(new { result = "success" });
		}

		public IActionResult Index()
        {
            var userName = GetUserName(HttpContext);
            // Store username to viewbag
            ViewBag.UserName = userName;

            // Verify if the user has a team
            var user = _context.Users.FirstOrDefault(u => u.Username == userName);
            if(user.NameOfTeam == "defaultName")
            {
                return RedirectToAction("Create", "Access");
            }

            // Store the nr of coins to viewbag
            ViewBag.wallet = getMoney();
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }

        public static string GetUserName(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var nameIdentifier = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return nameIdentifier;
            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        // Returns a view that displays news
        public IActionResult News()
        {
            // Get all news from the database
            var news = _context.News.ToList();

            var userName = GetUserName(HttpContext);
            // Store username to viewbag
            ViewBag.UserName = userName;
            // Store the nr of coins to viewbag
            ViewBag.wallet = getMoney();

            return View(news);
        }

        [HttpGet]
        // Returns a view that displays the leaderboard table
        public IActionResult Standings()
        {
            // Get all standings from the database
            List<Standing> standings = _context.Standings.ToList();
            
            // Order the standings by trophies
            standings = standings.OrderByDescending(s => s.Trophies).ToList();

            // Save the position in the list to the standing position
            int position = 1;
            foreach(Standing item in standings)
            {
                item.Position = position;
                position++;
            }

            foreach(Standing item in standings)
            {
                // Bind the user to his standing element
                User user = _context.Users.FirstOrDefault(u => u.UserId == item.UserId);
                item.User = user;
            }

            var userName = GetUserName(HttpContext);
            // Store username to viewbag
            ViewBag.UserName = userName;
            // Store the nr of coins to viewbag
            ViewBag.wallet = getMoney();

            return View(standings);
        }
    }
}
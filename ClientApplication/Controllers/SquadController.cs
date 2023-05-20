using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ClientApplication.Controllers
{
    public class SquadController : Controller
    {

        private readonly FootballDatabaseContext _context;

        public SquadController(FootballDatabaseContext context)
        {
            _context = context;
        }

        public int getMoney()
        {
            var userName = HomeController.GetUserName(HttpContext);
            var user = _context.Users.FirstOrDefault(u => u.Username == userName);

            return user.Coins;
        }

        [HttpGet]
        public async Task<IActionResult> Editor()
        {
            //Get the id of the user that is currently logged in
            try
            {
                var username = HomeController.GetUserName(HttpContext);
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                int userID = user.UserId;

                // Perform database query to retrieve players from Players table that are linked to the squad of the current user
                var players = _context.Players
                    .Include(p => p.TeamContracts)
                    .ThenInclude(tc => tc.Squad)
                    .ThenInclude(s => s.User)
                    .Where(p => p.TeamContracts.Any(tc => tc.SquadId == userID))
                    .Select(p => new Team
                    {
                        Players = new List<Player> { p },
                        Contracts = p.TeamContracts.Where(tc => tc.SquadId == userID).ToList()
                    })
                    .ToList();

                // Add nr of coins to the wallet
                ViewBag.wallet = getMoney();
                ViewBag.username = username;
                return View(players);
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("Login", "Access");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ClientApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ClientApplication.Controllers
{
    public class AccessController : Controller
    {

        private readonly FootballDatabaseContext _context;

        public AccessController(FootballDatabaseContext context)
        {
            _context = context;
        }

        // Redirects the user to the home page if they are already authenticated; otherwise, displays the login view
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Redirects the user to the home page if they are already authenticated; otherwise, displays the signup view
        public IActionResult Signup()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var today = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.Today = today;
            return View();
        }

        // Handles the login form submission
        [HttpPost]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            // Gets the user from the database based on the provided email
            var user = _context.Users.FirstOrDefault(u => u.Email == modelLogin.Email);
            if (user != null)
            {
                // Checks if the entered password matches the user's password
                if (modelLogin.Password == user.Password)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Username),
                        new Claim("OtherProperties", "Example Role")
                    };

                    // Creates an identity with the claims
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    // Configure authentication properties
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = modelLogin.RememberMe
                    };

                    // Signs in the user by creating a cookie-based authentication ticket
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                    if (user.NameOfTeam == "defaultName")
                    {
                        return RedirectToAction("Create", "Access");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ValidateMessage"] = "The password entered is wrong.";
                }
            }
            else
            {
                ViewData["ValidateMessage"] = "User not found.";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(Signup modelSignup)
        {
            // Check if a user with the provided username already exists
            var user = _context.Users.FirstOrDefault(u => u.Username == modelSignup.Username);
            if (user == null && modelSignup.Username != null)
            {
                // Check if the email is already in use
                var mail = _context.Users.FirstOrDefault(u => u.Email == modelSignup.Email);
                if (mail == null)
                {
                    // Check if the password and confirm password match
                    if (modelSignup.Password == modelSignup.ConfirmPassword && modelSignup.Password != null)
                    {
                        // Create a new user instance
                        User utilizator = new User();

                        // Set the properties of the new user
                        utilizator.Username = modelSignup.Username;
                        utilizator.Password = modelSignup.Password;
                        utilizator.Email = modelSignup.Email;
                        utilizator.DateOfBirth = modelSignup.DateOfBirth;

                        // Generate a new user ID starting from the last ID present in the table
                        // If the table is empty, the ID will be 1
                        if(_context.Users.Count() == 0)
                        {
                            utilizator.UserId = 1;
                        }
                        else
                        {
                            int maxId = _context.Users.Max(u => u.UserId);
                            utilizator.UserId = maxId + 1;
                        }

                        // Set default values for not null fields
                        utilizator.NameOfTeam = "defaultName";
                        utilizator.Coins = 1000;

                        // Add the user to the context and save changes to the database
                        _context.Users.Add(utilizator);
                        await _context.SaveChangesAsync();

                        // Create claims for the user
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, modelSignup.Username),
                            new Claim("OtherProperties", "Example Role")
                        };

                        // Create an identity with the claims
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Configure authentication properties
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = false
                        };

                        // Sign in the user
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                        // Redirect to the Create action of the Access controller (user chooses name of team)
                        return RedirectToAction("Create", "Access");
                    }
                    else
                    {
                        ViewData["ValidateMessage"] = "Passwords do not match or were not entered.";
                    }
                }
                else
                {
                    ViewData["ValidateMessage"] = "Email already in use.";
                }
            }
            else
            {
                ViewData["ValidateMessage"] = "User " + modelSignup.Username + " already exists. Please choose a different username!";
            }

            return View();
        }


        public IActionResult Create()
        {
            // Get the current user
            var username = HomeController.GetUserName(HttpContext);
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // Check if the user has already created a team
            if (user.NameOfTeam != "defaultName")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Squad modelSquad)
        {
            // Get the current user
            var username = HomeController.GetUserName(HttpContext);
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // Check if the user has already created a team
            if (user.NameOfTeam != "defaultName")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (modelSquad.SquadName != "defaultName" && modelSquad.SquadName != null)
                {
                    // Set the SquadId to be the same as the current user's UserId, set the UserID and the SquadName
                    Squad echipa = new Squad();
                    echipa.SquadId = user.UserId;
                    echipa.UserId = user.UserId;
                    echipa.SquadName = modelSquad.SquadName;

                    // Add the Squad to the Database and to the user's Squads, change nameOfTeam field inside Users
                    _context.Squads.Add(echipa);
                    user.NameOfTeam = modelSquad.SquadName;
                    user.Squads.Add(echipa);

                    // Select all players (both outfield players and goalkeepers)
                    var allPlayers = await _context.Players.ToListAsync();

                    // Shuffle the list of players randomly
                    var random = new Random();
                    var shuffledPlayers = allPlayers.OrderBy(p => random.Next()).ToList();

                    // Select the goalkeeper and add the goalkeeper to the team and to the database on the 1st position
                    var goalkeeper = shuffledPlayers.FirstOrDefault(p => p.Position.TrimEnd() == "GK");

                    // Team needs to have 18 players upon creation: 1 legendary player, 4 rare players, and 13 common players
                    // We choose the other players based on the quality of the goalkeeper
                    var legendaryPlayersCount = 1;
                    var rarePlayersCount = 4;
                    var commonPlayersCount = 13;

                    if (goalkeeper != null)
                    {
                        if (goalkeeper.OverallRank >= 85)
                        {
                            // If the goalkeeper is Legendary, we skip selecting a Legendary player
                            legendaryPlayersCount--;
                        }
                        else if (goalkeeper.OverallRank >= 80 && goalkeeper.OverallRank < 85)
                        {
                            // If the goalkeeper is Rare, we choose one less Rare player
                            rarePlayersCount--;
                        }
                        else
                        {
                            // If the goalkeeper is Common, we choose one less Common player
                            commonPlayersCount--;
                        }
                    }

                    // Select additional players based on the modified counts
                    var legendaryPlayers = shuffledPlayers.Where(p => p.OverallRank >= 85 && p.Position.TrimEnd() != "GK").Take(legendaryPlayersCount).ToList();
                    var rarePlayers = shuffledPlayers.Where(p => p.OverallRank >= 80 && p.OverallRank < 85 && p.Position.TrimEnd() != "GK").Take(rarePlayersCount).ToList(); 
                    var commonPlayers = shuffledPlayers.Where(p => p.OverallRank < 80 && p.Position.TrimEnd() != "GK").Take(commonPlayersCount).ToList();

                    // Combine the selected players into a single list
                    var selectedPlayers = legendaryPlayers.Concat(rarePlayers).Concat(commonPlayers).ToList();

                    // Add the selected players to the team and database
                    int position = 1;

                    // If the contracts are empty, we set the maxId to 0, otherwise we set it to the maximum ContractId
                    int maxId = 0;
                    if(_context.TeamContracts.Count() == 0)
                    {
                        maxId = 0;
                    }
                    else
                    {
                        maxId = _context.TeamContracts.Max(u => u.ContractId);
                    }
                    var goalkeeperContract = new TeamContract
                    {
                        ContractId = ++maxId,
                        PlayerId = goalkeeper.PlayerId,
                        SquadId = echipa.SquadId,
                        ShirtNumber = null,
                        IsCaptain = false,
                        Position = position++
                    };
                    echipa.TeamContracts.Add(goalkeeperContract);
                    _context.TeamContracts.Add(goalkeeperContract);
                    await _context.SaveChangesAsync();

                    foreach (var player in selectedPlayers)
                    {
                        var contract = new TeamContract
                        {
                            ContractId = ++maxId,
                            PlayerId = player.PlayerId,
                            SquadId = echipa.SquadId,
                            ShirtNumber = null,
                            IsCaptain = false,
                            Position = position++
                        };
                        echipa.TeamContracts.Add(contract);
                        _context.TeamContracts.Add(contract);
                        await _context.SaveChangesAsync();
                    }

                    // Redirect to the index page with a success message
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ValidateMessage"] = "Please choose a valid name first!";
                }
            }
            return View();
        }
    }
}
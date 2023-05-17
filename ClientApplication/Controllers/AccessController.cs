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

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Signup()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            var today = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.Today = today;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            //gets the user from the db
            var user = _context.Users.FirstOrDefault(u => u.Email == modelLogin.Email);

            if (user != null)
            {
                //check if password matches
                if (modelLogin.Password == user.Password)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Username),//new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                        new Claim("OtherProperties", "Example Role")
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = modelLogin.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    if(user.NameOfTeam == "defaultName")
                    {
                        ViewData["ValidateMessage"] = "You need to create a team!";
                        return RedirectToAction("Create", "Access");
                    }

                    return RedirectToAction("Index", "Home");

                }
                else ViewData["ValidateMessage"] = "The password entered is wrong.";
            }
            else ViewData["ValidateMessage"] = "User not found.";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(Signup modelSignup)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == modelSignup.Username);
            if (user == null && modelSignup.Username != null)
            {
                var mail = _context.Users.FirstOrDefault(u => u.Email == modelSignup.Email);
                if (mail == null)
                {
                    if (modelSignup.Password == modelSignup.ConfirmPassword && modelSignup.Password != null)
                    {
                        //de aici dam push in db
                        User utilizator = new User();

                        utilizator.Username = modelSignup.Username;
                        utilizator.Password = modelSignup.Password;
                        utilizator.Email = modelSignup.Email;
                        utilizator.DateOfBirth = modelSignup.DateOfBirth;

                        //provide other necessary info 
                        //Generate a new id
                        // If no users in the database, set the id to 1, else set it to the max id + 1
                        if (_context.Users.Count() == 0) utilizator.UserId = 1;
                        else
                        {
                            int maxId = _context.Users.Max(u => u.UserId);
                            utilizator.UserId = maxId + 1;
                        }


                        //default values for not null fields
                        utilizator.NameOfTeam = "defaultName";
                        utilizator.Coins = 1000;

                        _context.Users.Add(utilizator);
                        await _context.SaveChangesAsync();

                        List<Claim> claims = new List<Claim>()
                        {
                            //new Claim(ClaimTypes.NameIdentifier, modelSignup.Email),
                            new Claim(ClaimTypes.NameIdentifier, modelSignup.Username),
                            new Claim("OtherProperties", "Example Role")
                        };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = false
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        return RedirectToAction("Create", "Access");

                    }
                    else ViewData["ValidateMessage"] = "Passwords do not match or were not entered.";
                }
                else ViewData["ValidateMessage"] = "Email already in use.";
            }
            else ViewData["ValidateMessage"] = "User " + modelSignup.Username + " already exists. Please choose a different username!";

            return View();
        }

        public IActionResult Create()
        {
            // Get the current user
            var username = HomeController.GetUserName(HttpContext);
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user.NameOfTeam != "defaultName")
            {
                ViewData["ValidateMessage"] = "You already have a team!";
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

            // Set the SquadId to be the same as the current user's UserId, set the UserID and the SquadName
            Squad echipa = new();
            echipa.SquadId = user.UserId;
            echipa.UserId = user.UserId;
            echipa.SquadName = modelSquad.SquadName;

            // If no team name was inserted, redirect to the same page
            if (modelSquad.SquadName == null)
            {
                ViewData["ValidateMessage"] = "Please enter a team name!";
                return View();
            }

            // Add the Squad to the Database and to the user's Squads, change nameOfTeam field inside Users
            _context.Squads.Add(echipa);
            user.NameOfTeam = modelSquad.SquadName;
            user.Squads.Add(echipa);

            // Select 1 legendary player, 3 rare players, and 12 common players
            // Randomize them and store them in variables
            var legendaryPlayers = await _context.Players.Where(p => p.OverallRank >= 85).OrderBy(p => Guid.NewGuid()).Take(1).ToListAsync();
            var rarePlayers = await _context.Players.Where(p => p.OverallRank >= 80 && p.OverallRank < 85).OrderBy(p => Guid.NewGuid()).Take(4).ToListAsync();
            var commonPlayers = await _context.Players.Where(p => p.OverallRank < 80).OrderBy(p => Guid.NewGuid()).Take(13).ToListAsync();

            // Create team contracts for each player and add them to the new squad
            int i = 1;
            foreach (var legendaryPlayer in legendaryPlayers)
            {
                // Generate new id
                // If no contracts in the database, set the id to 1, else set it to the max id + 1
                int maxId;
                if (_context.TeamContracts.Count() == 0)
                {
                    maxId = 0;
                }
                else
                {
                    maxId = _context.TeamContracts.Max(u => u.ContractId);
                }

                var contract = new TeamContract
                {
                    ContractId = maxId + 1,
                    PlayerId = legendaryPlayer.PlayerId,
                    SquadId = echipa.SquadId,
                    ShirtNumber = null,
                    IsCaptain = false,
                    Position = i++
                };
                echipa.TeamContracts.Add(contract);
                _context.TeamContracts.Add(contract);
                await _context.SaveChangesAsync();
            }

            foreach (var rarePlayer in rarePlayers)
            {
                // Generate new id
                int maxId = _context.TeamContracts.Max(u => u.ContractId);
                var contract = new TeamContract
                {
                    ContractId = maxId + 1,
                    PlayerId = rarePlayer.PlayerId,
                    SquadId = echipa.SquadId,
                    ShirtNumber = null,
                    IsCaptain = false,
                    Position = i++
                };
                echipa.TeamContracts.Add(contract);
                _context.TeamContracts.Add(contract);
                await _context.SaveChangesAsync();
            }

            foreach (var commonPlayer in commonPlayers)
            {
                // Generate new id
                int maxId = _context.TeamContracts.Max(u => u.ContractId);
                var contract = new TeamContract
                {
                    ContractId = maxId + 1,
                    PlayerId = commonPlayer.PlayerId,
                    SquadId = echipa.SquadId,
                    ShirtNumber = null,
                    IsCaptain = false,
                    Position = i++
                };
                echipa.TeamContracts.Add(contract);
                _context.TeamContracts.Add(contract);
                await _context.SaveChangesAsync();
            }
            // Save changes to the database
            //await _context.SaveChangesAsync();

            // Redirect to the index page with a success message
            ViewData["ValidateMessage"] = "Team created successfully!";
            return RedirectToAction("Index", "Home");
        }

    }
}
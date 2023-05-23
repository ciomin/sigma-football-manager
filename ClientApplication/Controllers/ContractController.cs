using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientApplication.Controllers
{
    public class ContractController : Controller
    {
        private readonly FootballDatabaseContext _context;

        public ContractController(FootballDatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePositions(int currentContractId, int droppedContractId)
        {
            // Try to see if user is logged in
            try
            {
                var username = HomeController.GetUserName(HttpContext);

                // Find the contracts to swap by their IDs
                var currentContract = _context.TeamContracts.SingleOrDefault(c => c.ContractId == currentContractId);
                var droppedContract = _context.TeamContracts.SingleOrDefault(c => c.ContractId == droppedContractId);

                if (currentContract == null || droppedContract == null)
                {
                    return BadRequest("One or both of the contracts could not be found.");
                }

                // Swap the positions of the contracts
                var tempPosition = currentContract.Position;
                currentContract.Position = droppedContract.Position;
                droppedContract.Position = tempPosition;

                // Save the changes to the database
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occurred while saving the changes: {ex.Message}");
                }

                // Return the same page so I can keep the login redirect in case the user is not logged in
                return RedirectToAction("Editor", "Squad");
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("Login", "Access");
            }
        }
    }
}

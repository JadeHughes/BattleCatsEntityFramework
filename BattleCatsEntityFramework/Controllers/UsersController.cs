using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BattleCatsEntityFramework.Data;
using BattleCatsEntityFramework.Models;
using BattleCatsEntityFramework.DTOs;

namespace BattleCatsEntityFramework.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly BattleCatsContext _context;

        public UsersController(BattleCatsContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return Conflict("Username already exists.");

            var user = new User
            {
                Username = request.Username,
                Password = request.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            return Ok("Login successful.");
        }



        [HttpGet("{username}/cards")]
        public async Task<IActionResult> GetUserCards(string username)
        {
            var user = await _context.Users
                .Include(u => u.UserCards)
                .ThenInclude(uc => uc.Card)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
                return NotFound("User not found.");

            var cards = user.UserCards.Select(uc => new BattleCatsCard
            {
                Id = uc.Card.Id,
                Name = uc.Card.Name,
                ChonkRating = uc.Card.ChonkRating,
                SpecialAttack = uc.Card.SpecialAttack,
                SpecialAttackPower = uc.Card.SpecialAttackPower,
                Fluffiness = uc.Card.Fluffiness,
                Attitude = uc.Card.Attitude
            }).ToList();

            return Ok(cards); //returns clean JSON array
        }

        [HttpPost("{username}/mystery-box")]
        public async Task<IActionResult> OpenMysteryBox(string username)
        {
            var user = await _context.Users
                .Include(u => u.UserCards)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
                return NotFound("User not found.");

            var rng = new Random();
            int roll = rng.Next(1, 7); // 1 to 6

            if (roll == 1 || roll == 6)
            {
                var allCards = await _context.BattleCatsCards.ToListAsync();

                if (!allCards.Any())
                    return StatusCode(500, "No cards available to award.");

                var randomCard = allCards[rng.Next(allCards.Count)];

                // Check if the user already owns it
                bool alreadyHasCard = user.UserCards.Any(uc => uc.CardId == randomCard.Id);
                if (!alreadyHasCard)
                {
                    user.UserCards.Add(new UserCard
                    {
                        UserId = user.Id,
                        CardId = randomCard.Id
                    });

                    await _context.SaveChangesAsync();
                    return Ok(new { message = $"You won a card: {randomCard.Name}!" });
                }

                return Ok(new { message = $"You rolled a winning number but already own {randomCard.Name}!" });
            }

            return Ok(new { message = "Sorry, nothing this time. Try again later!" });
        }

        [HttpDelete("{username}/remove-random-card")]
        public async Task<IActionResult> RemoveRandomCard(string username)
        {
            var user = await _context.Users
                .Include(u => u.UserCards)
                .ThenInclude(uc => uc.Card)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
                return NotFound("User not found.");

            if (user.UserCards.Count == 0)
                return BadRequest("User has no cards to remove.");

            var rng = new Random();
            var cardToRemove = user.UserCards[rng.Next(user.UserCards.Count)];

            user.UserCards.Remove(cardToRemove);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"One of your cards ({cardToRemove.Card.Name}) has been removed." });
        }



    }
}

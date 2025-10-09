using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Data;

namespace WebApiAngular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // wszystkie akcje tylko dla admina
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 🔒 testowy endpoint 
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("🔒 Only admins can see this");
        }

        // 📋 lista użytkowników
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // usuwanie użytkownika
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // pobierz aktualnego admina z tokena
            var currentUsername = User.Identity?.Name ?? 
                                  User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found");

            // zabezpieczenie: admin nie może usunąć samego siebie
            if (user.Username == currentUsername)
                return BadRequest("Admin cannot delete their own account");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User '{user.Username}' deleted successfully" });
        }
    }
}
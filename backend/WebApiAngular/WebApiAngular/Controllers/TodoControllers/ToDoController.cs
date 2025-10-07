using DefaultNamespace.ToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Data;
using WebApiAngular.Dtos;

namespace WebApiAngular.Controllers.TodoControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // wymagany token dla wszystkich operacji
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        // Pobranie aktualnego użytkownika po claimie "username"
        private int GetCurrentUserId()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not found");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                throw new Exception("User not found");

            return user.Id;
        }

        // GET: api/Todo/mytodos
        [HttpGet("mytodos")]
        public async Task<IActionResult> GetMyTodos()
        {
            var userId = GetCurrentUserId();

            var todos = await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Completed = t.Completed
                })
                .ToListAsync();

            return Ok(todos);
        }

        // POST: api/Todo/mytodos
        [HttpPost("mytodos")]
        public async Task<IActionResult> CreateMyTodo([FromBody] TodoItem todo)
        {
            todo.UserId = GetCurrentUserId();
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMyTodos), new { id = todo.Id }, todo);
        }

        // PUT: api/Todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoItem todo)
        {
            var userId = GetCurrentUserId();
            var existing = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (existing == null) return NotFound();

            existing.Title = todo.Title;
            existing.Completed = todo.Completed;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var existing = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (existing == null) return NotFound();

            _context.TodoItems.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

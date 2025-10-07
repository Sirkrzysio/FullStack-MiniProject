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
        public async Task<IActionResult> CreateMyTodo([FromBody] TodoItemDto dto)
        {
            var userId = GetCurrentUserId();

            var todo = new TodoItem
            {
                Title = dto.Title,
                Completed = dto.Completed,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            var resultDto = new TodoItemDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Completed = todo.Completed
            };

            return CreatedAtAction(nameof(GetMyTodos), new { id = todo.Id }, resultDto);
        }


        // PUT: api/Todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoItemDto dto)
        {
            var userId = GetCurrentUserId();

           
            var existing = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (existing == null)
                return NotFound(); 

            // aktualizacja pól
            existing.Title = dto.Title;
            existing.Completed = dto.Completed;

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

            if (existing == null)
                return NotFound(); 

            _context.TodoItems.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

    }
}

using DefaultNamespace.ToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Data;

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

        // GET: api/Todo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todos = await _context.TodoItems.ToListAsync();
            return Ok(todos);
        }

        // GET: api/Todo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoItem todo)
        {
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }

        // PUT: api/Todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoItem todo)
        {
            var existing = await _context.TodoItems.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = todo.Title;
            existing.IsCompleted = todo.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
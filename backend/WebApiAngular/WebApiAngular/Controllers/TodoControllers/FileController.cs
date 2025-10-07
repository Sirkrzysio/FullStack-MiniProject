using System.Text.Json;
using DefaultNamespace.ToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Data;
using WebApiAngular.Dtos;

namespace WebApiAngular.Controllers.TodoControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    // Pobranie aktualnego użytkownika na podstawie claimu "username"
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

    // Upload pliku przypisanego do użytkownika
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Empty file");

        var userId = GetCurrentUserId();

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;
        
        // wczytywanie pliku
        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            Content = ms.ToArray(),
            UploadedAt = DateTime.UtcNow,
            UserId = userId
        };
        _context.UploadedFiles.Add(uploadedFile);
        await _context.SaveChangesAsync();

        
        ms.Position = 0;
        //dodawanie rekordow
        List<TodoItemDto>? todoDtos;
        try
        {
            using var reader = new StreamReader(ms);
            var json = await reader.ReadToEndAsync();
            todoDtos = System.Text.Json.JsonSerializer.Deserialize<List<TodoItemDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return BadRequest("Invalid JSON");
        }

        if (todoDtos == null || todoDtos.Count == 0)
            return BadRequest("No todos in file");

        var todos = todoDtos.Select(dto => new TodoItem
        {
            Title = dto.Title,
            Completed = dto.Completed, 
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _context.TodoItems.AddRange(todos);
        await _context.SaveChangesAsync();

        return Ok(new { Count = todos.Count });
    }

    // Pobranie wszystkich plików aktualnego użytkownika
    [HttpGet("myfiles")]
    public async Task<IActionResult> GetMyFiles()
    {
        var userId = GetCurrentUserId();

        var files = await _context.UploadedFiles
            .Where(f => f.UserId == userId)
            .Select(f => new { f.Id, f.FileName, f.UploadedAt })
            .ToListAsync();

        return Ok(files);
    }
}


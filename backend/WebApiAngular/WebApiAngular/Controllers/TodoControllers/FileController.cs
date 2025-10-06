using DefaultNamespace.ToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Data;

namespace WebApiAngular.Controllers.TodoControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // tylko zalogowani
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FilesController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) throw new Exception("User not found");
        return user.Id;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Empty file");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var userId = GetCurrentUserId();

        var uploaded = new UploadedFile
        {
            FileName = file.FileName,
            Content = ms.ToArray(),
            UserId = userId
        };

        _context.UploadedFiles.Add(uploaded);
        await _context.SaveChangesAsync();

        return Ok(uploaded);
    }

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

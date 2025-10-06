using DefaultNamespace;
using DefaultNamespace.ToDo;
using Microsoft.EntityFrameworkCore;

namespace WebApiAngular.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
        
        public DbSet<UploadedFile> UploadedFiles { get; set; } = null!;


    }
    
}
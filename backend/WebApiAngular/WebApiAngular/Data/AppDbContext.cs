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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Relacja User -> TodoItems (usuwa todosy przy kasowaniu usera)
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.User)
                .WithMany() 
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja User -> UploadedFiles (usuwa pliki przy kasowaniu usera)
            modelBuilder.Entity<UploadedFile>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
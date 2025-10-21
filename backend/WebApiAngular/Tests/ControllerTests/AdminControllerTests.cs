using System.Security.Claims;
using DefaultNamespace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Controllers;
using WebApiAngular.Data;
using Xunit;

namespace Tests.ControllerTests
{
    public class AdminControllerTests
    {
        private readonly AppDbContext _context;
        private readonly AdminController _controller;
        private readonly User _adminUser;

        public AdminControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Admin user
            _adminUser = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = "hash",
                Role = "Admin"
            };
            _context.Users.Add(_adminUser);
            _context.SaveChanges();

            _controller = new AdminController(_context);

            // Mock admin authentication
            var claims = new List<Claim>
            {
                new Claim("username", "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetSecret_AdminAuthenticated_ReturnsSecretMessage()
        {
            // Act
            var result = _controller.GetSecret();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("🔒 Only admins can see this", okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsAllUsersWithoutPasswords()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "user1", Email = "user1@test.com", PasswordHash = "hash1", Role = "User" },
                new User { Username = "user2", Email = "user2@test.com", PasswordHash = "hash2", Role = "User" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = okResult.Value as IEnumerable<object>;
            Assert.NotNull(users);
            Assert.Equal(3, users.Count()); 

            // Sprawdź że nie ma PasswordHash w response
            var firstUser = users.First();
            var properties = firstUser.GetType().GetProperties().Select(p => p.Name);
            Assert.Contains("Username", properties);
            Assert.Contains("Email", properties);
            Assert.Contains("Role", properties);
            Assert.DoesNotContain("PasswordHash", properties);
        }

        [Fact]
        public async Task DeleteUser_ExistingUser_DeletesSuccessfully()
        {
            var userToDelete = new User
            {
                Username = "deleteme",
                Email = "delete@test.com",
                PasswordHash = "hash",
                Role = "User"
            };
            _context.Users.Add(userToDelete);
            await _context.SaveChangesAsync();
            var userId = userToDelete.Id;

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            var deleted = await _context.Users.FindAsync(userId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteUser_NonExistent_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteUser(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteUser_AdminDeletingThemselves_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteUser(_adminUser.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Admin cannot delete their own account", badRequestResult.Value);
            
            var stillExists = await _context.Users.FindAsync(_adminUser.Id);
            Assert.NotNull(stillExists);
        }

        [Fact]
        public async Task DeleteUser_ChecksCurrentUserFromClaims()
        {
            var anotherAdmin = new User
            {
                Username = "admin2",
                Email = "admin2@test.com",
                PasswordHash = "hash",
                Role = "Admin"
            };
            _context.Users.Add(anotherAdmin);
            await _context.SaveChangesAsync();

            // Act 
            var result = await _controller.DeleteUser(anotherAdmin.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            
            var deleted = await _context.Users.FindAsync(anotherAdmin.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetAllUsers_IncludesUserRoles()
        {
            
            _context.Users.Add(new User
            {
                Username = "regularuser",
                Email = "regular@test.com",
                PasswordHash = "hash",
                Role = "User"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = okResult.Value as IEnumerable<object>;

            Assert.NotNull(users);
            
            var adminInList = users.FirstOrDefault(u =>
                u.GetType().GetProperty("Username")!.GetValue(u)!.ToString() == "admin"
            );
            var userInList = users.FirstOrDefault(u =>
                u.GetType().GetProperty("Username")!.GetValue(u)!.ToString() == "regularuser"
            );

            Assert.NotNull(adminInList);
            Assert.NotNull(userInList);
            
            var adminRole = adminInList.GetType().GetProperty("Role")!.GetValue(adminInList)!.ToString();
            var userRole = userInList.GetType().GetProperty("Role")!.GetValue(userInList)!.ToString();

            Assert.Equal("Admin", adminRole);
            Assert.Equal("User", userRole);

        }
    }
}
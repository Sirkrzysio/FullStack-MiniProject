using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApiAngular.Controllers;
using WebApiAngular.Data;
using Xunit;

namespace WebApiAngular.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AppDbContext _context;
        private readonly AuthController _controller;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            // In-Memory Database setup
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Mock configuration dla JWT
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "SuperSecretKeyThatIsAtLeast32CharactersLongForTesting123456"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpiresInMinutes", "60"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _controller = new AuthController(_context, _configuration);
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@test.com",
                Password = "Test123!"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            
            // Sprawdź czy user został dodany do bazy
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(user);
            Assert.Equal("test@test.com", user.Email);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task Register_DuplicateUsername_ReturnsBadRequest()
        {
            var existingUser = new User
            {
                Username = "duplicate",
                Email = "existing@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "User"
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var registerDto = new RegisterDto
            {
                Username = "duplicate",
                Email = "new@test.com",
                Password = "Test123!"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokenAndUserInfo()
        {
            var user = new User
            {
                Username = "loginuser",
                Email = "login@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            
            // Sprawdź czy zwrócono token, username i role
            var token = value?.GetType().GetProperty("token")?.GetValue(value);
            var username = value?.GetType().GetProperty("username")?.GetValue(value);
            var role = value?.GetType().GetProperty("role")?.GetValue(value);

            Assert.NotNull(token);
            Assert.Equal("loginuser", username);
            Assert.Equal("User", role);
        }

        [Fact]
        public async Task Login_InvalidUsername_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto
            {
                Username = "nonexistent",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateAdmin_NoExistingAdmin_CreatesAdmin()
        {
            // Act
            var result = await _controller.CreateAdmin();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Admin created", okResult.Value);

            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.Equal("Admin", admin.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify("Admin123!", admin.PasswordHash));
        }

        [Fact]
        public async Task CreateAdmin_AdminAlreadyExists_ReturnsBadRequest()
        {
            var existingAdmin = new User
            {
                Username = "admin",
                Email = "admin@local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "Admin"
            };
            _context.Users.Add(existingAdmin);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.CreateAdmin();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Admin already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_PasswordIsHashed()
        {
            var registerDto = new RegisterDto
            {
                Username = "hashtest",
                Email = "hash@test.com",
                Password = "PlainPassword123"
            };

            // Act
            await _controller.Register(registerDto);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "hashtest");
            Assert.NotNull(user);
            
            Assert.NotEqual("PlainPassword123", user.PasswordHash);
            
            Assert.True(BCrypt.Net.BCrypt.Verify("PlainPassword123", user.PasswordHash));
        }
    }
}
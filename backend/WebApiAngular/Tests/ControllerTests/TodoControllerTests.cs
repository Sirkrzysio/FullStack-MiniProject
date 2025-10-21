using System.Security.Claims;
using DefaultNamespace;
using DefaultNamespace.ToDo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.Controllers.TodoControllers;
using WebApiAngular.Data;
using WebApiAngular.Dtos;
using Xunit;

namespace WebApiAngular.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly AppDbContext _context;
        private readonly TodoController _controller;
        private readonly User _testUser;

        public TodoControllerTests()
        {
            // In-Memory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Utwórz testowego usera
            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Role = "User"
            };
            _context.Users.Add(_testUser);
            _context.SaveChanges();

            _controller = new TodoController(_context);

            // Mock authenticated user - KLUCZOWE dla autoryzacji!
            var claims = new List<Claim>
            {
                new Claim("username", "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetMyTodos_ReturnsOnlyUserTodos()
        {
            var otherUser = new User
            {
                Id = 2,
                Username = "otheruser",
                Email = "other@test.com",
                PasswordHash = "hash",
                Role = "User"
            };
            _context.Users.Add(otherUser);

            _context.TodoItems.AddRange(
                new TodoItem { Title = "My Todo 1", UserId = _testUser.Id, Completed = false },
                new TodoItem { Title = "My Todo 2", UserId = _testUser.Id, Completed = true },
                new TodoItem { Title = "Other Todo", UserId = otherUser.Id, Completed = false }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMyTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<List<TodoItemDto>>(okResult.Value);
            
            Assert.Equal(2, todos.Count);
            Assert.All(todos, todo => Assert.Contains("My Todo", todo.Title));
        }

        [Fact]
        public async Task GetMyTodos_EmptyList_ReturnsEmptyArray()
        {
            // Act
            var result = await _controller.GetMyTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<List<TodoItemDto>>(okResult.Value);
            Assert.Empty(todos);
        }

        [Fact]
        public async Task CreateMyTodo_ValidDto_ReturnsTodoWithId()
        {
            var dto = new TodoItemDto
            {
                Title = "New Task",
                Completed = false
            };

            // Act
            var result = await _controller.CreateMyTodo(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedDto = Assert.IsType<TodoItemDto>(createdResult.Value);
            
            Assert.Equal("New Task", returnedDto.Title);
            Assert.False(returnedDto.Completed);
            Assert.True(returnedDto.Id > 0);

            // Sprawdź w bazie
            var dbTodo = await _context.TodoItems.FindAsync((int)returnedDto.Id);
            Assert.NotNull(dbTodo);
            Assert.Equal(_testUser.Id, dbTodo.UserId);
        }

        [Fact]
        public async Task Update_ExistingTodo_UpdatesSuccessfully()
        {
            var todo = new TodoItem
            {
                Title = "Original Title",
                Completed = false,
                UserId = _testUser.Id
            };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            var updateDto = new TodoItemDto
            {
                Title = "Updated Title",
                Completed = true
            };

            // Act
            var result = await _controller.Update(todo.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updated = await _context.TodoItems.FindAsync(todo.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
            Assert.True(updated.Completed);
        }

        [Fact]
        public async Task Update_NonExistentTodo_ReturnsNotFound()
        {
            var updateDto = new TodoItemDto
            {
                Title = "Updated",
                Completed = true
            };

            // Act
            var result = await _controller.Update(999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_OtherUserTodo_ReturnsNotFound()
        {
            var otherUser = new User
            {
                Id = 3,
                Username = "otheruser",
                Email = "other@test.com",
                PasswordHash = "hash",
                Role = "User"
            };
            _context.Users.Add(otherUser);

            var otherTodo = new TodoItem
            {
                Title = "Other's Todo",
                Completed = false,
                UserId = otherUser.Id
            };
            _context.TodoItems.Add(otherTodo);
            await _context.SaveChangesAsync();

            var updateDto = new TodoItemDto
            {
                Title = "Hacked!",
                Completed = true
            };

            // Act - próba aktualizacji cudzego todo
            var result = await _controller.Update(otherTodo.Id, updateDto);

            // Assert 
            Assert.IsType<NotFoundResult>(result);
            
            var unchanged = await _context.TodoItems.FindAsync(otherTodo.Id);
            Assert.Equal("Other's Todo", unchanged!.Title);
        }

        [Fact]
        public async Task Delete_ExistingTodo_DeletesSuccessfully()
        {
            var todo = new TodoItem
            {
                Title = "To Delete",
                Completed = false,
                UserId = _testUser.Id
            };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
            var todoId = todo.Id;

            // Act
            var result = await _controller.Delete(todoId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deleted = await _context.TodoItems.FindAsync(todoId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Delete_NonExistentTodo_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_OtherUserTodo_ReturnsNotFound()
        {
            var otherUser = new User
            {
                Id = 4,
                Username = "other",
                Email = "other@test.com",
                PasswordHash = "hash",
                Role = "User"
            };
            _context.Users.Add(otherUser);

            var otherTodo = new TodoItem
            {
                Title = "Protected Todo",
                UserId = otherUser.Id
            };
            _context.TodoItems.Add(otherTodo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(otherTodo.Id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            
            var stillExists = await _context.TodoItems.FindAsync(otherTodo.Id);
            Assert.NotNull(stillExists);
        }

        [Fact]
        public async Task GetMyTodos_OrdersByCreatedAtDescending()
        {
            var older = new TodoItem
            {
                Title = "Older",
                UserId = _testUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            var newer = new TodoItem
            {
                Title = "Newer",
                UserId = _testUser.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.TodoItems.AddRange(older, newer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMyTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<List<TodoItemDto>>(okResult.Value);
            
            Assert.Equal(2, todos.Count);
            Assert.Equal("Newer", todos[0].Title); 
            Assert.Equal("Older", todos[1].Title);
        }
    }
}
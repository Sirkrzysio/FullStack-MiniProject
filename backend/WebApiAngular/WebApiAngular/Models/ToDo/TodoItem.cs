using System.Text.Json.Serialization;

namespace DefaultNamespace.ToDo;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("completed")] // mapowanie JSON.completed → DTO.Completed
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
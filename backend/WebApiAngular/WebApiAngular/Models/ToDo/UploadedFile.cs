using System.Text.Json.Serialization;

namespace DefaultNamespace.ToDo;

public class UploadedFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!; 
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; } = null!;
}

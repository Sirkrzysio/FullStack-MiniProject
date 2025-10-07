using System.Text.Json.Serialization;

namespace WebApiAngular.Dtos
{
    public class TodoItemDto
    {
        public long Id { get; set; }    
        public string Title { get; set; } = null!;
        [JsonPropertyName("completed")] // mapowanie JSON.completed → DTO.Completed
        public bool Completed { get; set; }
    }
}
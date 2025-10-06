using System.ComponentModel.DataAnnotations;

namespace RoomieMatch.Api.Models;

public record MessageThreadResponse(Guid Id, Guid RoomId, Guid OwnerId, Guid SeekerId, DateTime CreatedAt, DateTime? LastMessageAt);
public record MessageResponse(Guid Id, Guid ThreadId, Guid SenderId, string Content, DateTime CreatedAt);

public class CreateThreadRequest
{
    [Required]
    public Guid RoomId { get; set; }
    [Required]
    public Guid OwnerId { get; set; }
    [Required]
    public Guid SeekerId { get; set; }
}

public class SendMessageRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

using System;

namespace RoomieMatch.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public MessageThread Thread { get; set; } = default!;
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = default!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

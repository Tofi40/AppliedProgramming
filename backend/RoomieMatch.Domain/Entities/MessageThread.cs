using System;
using System.Collections.Generic;

namespace RoomieMatch.Domain.Entities;

public class MessageThread
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = default!;
    public Guid SeekerId { get; set; }
    public User Seeker { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

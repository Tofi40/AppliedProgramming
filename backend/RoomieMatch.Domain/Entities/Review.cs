using System;

namespace RoomieMatch.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public User Reviewer { get; set; } = default!;
    public Guid RevieweeId { get; set; }
    public User Reviewee { get; set; } = default!;
    public int Rating { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

using System;

namespace RoomieMatch.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }
    public Guid SeekerId { get; set; }
    public User Seeker { get; set; } = default!;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = default!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;
    public double CompatibilityScore { get; set; }
    public string Rationale { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

using System;

namespace RoomieMatch.Domain.Entities;

public enum BookingRequestStatus
{
    Pending,
    Approved,
    Declined,
    Withdrawn
}

public class BookingRequest
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;
    public Guid SeekerId { get; set; }
    public User Seeker { get; set; } = default!;
    public BookingRequestStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

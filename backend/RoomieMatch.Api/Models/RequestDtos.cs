using System.ComponentModel.DataAnnotations;
using RoomieMatch.Domain.Entities;

namespace RoomieMatch.Api.Models;

public record BookingRequestResponse(Guid Id, Guid RoomId, Guid SeekerId, BookingRequestStatus Status, string? Note, DateTime CreatedAt, DateTime UpdatedAt);

public class CreateBookingRequest
{
    [Required]
    public Guid RoomId { get; set; }
    public string? Note { get; set; }
}

public class UpdateBookingRequest
{
    [Required]
    public BookingRequestStatus Status { get; set; }
    public string? Note { get; set; }
}

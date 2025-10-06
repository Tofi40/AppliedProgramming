using System;
using System.Collections.Generic;

namespace RoomieMatch.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = default!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public DateOnly AvailableFrom { get; set; }
    public int MinTermMonths { get; set; }
    public int PricePerMonthCents { get; set; }
    public bool BillsIncluded { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> HouseRules { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();
}

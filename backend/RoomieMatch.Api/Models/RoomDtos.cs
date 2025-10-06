using System.ComponentModel.DataAnnotations;

namespace RoomieMatch.Api.Models;

public record RoomResponse(Guid Id, string Title, string Description, string City, string Country, int PricePerMonthCents, bool BillsIncluded, bool IsPublished, double? CompatibilityScore, string[]? Rationale);

public class CreateRoomRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public DateOnly AvailableFrom { get; set; }
    [Range(1, 36)]
    public int MinTermMonths { get; set; }
    [Range(1, int.MaxValue)]
    public int PricePerMonthCents { get; set; }
    public bool BillsIncluded { get; set; }
    public string[] Amenities { get; set; } = Array.Empty<string>();
    public string[] HouseRules { get; set; } = Array.Empty<string>();
    public string[] Photos { get; set; } = Array.Empty<string>();
}

public class UpdateRoomRequest : CreateRoomRequest
{
    public bool? IsPublished { get; set; }
}

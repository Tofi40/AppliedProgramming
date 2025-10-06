using System.ComponentModel.DataAnnotations;

namespace RoomieMatch.Api.Models;

public record UserResponse(Guid Id, string Email, string DisplayName, string Role, string? AvatarUrl, string City, string Country, bool OnboardingComplete);

public class UpdateUserRequest
{
    [MaxLength(256)]
    public string? DisplayName { get; set; }
    [MaxLength(1024)]
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string[]? Interests { get; set; }
    public string[]? Habits { get; set; }
    public string[]? Personality { get; set; }
}

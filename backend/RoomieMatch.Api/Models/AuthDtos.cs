using System.ComponentModel.DataAnnotations;

namespace RoomieMatch.Api.Models;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string DisplayName,
    [Required] string Role
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

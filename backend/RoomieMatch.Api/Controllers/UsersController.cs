using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoomieMatch.Api.Models;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UsersController(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        return ToResponse(user);
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe(UpdateUserRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            user.DisplayName = request.DisplayName;
        }

        if (!string.IsNullOrWhiteSpace(request.Bio))
        {
            user.Bio = request.Bio;
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            user.City = request.City;
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            user.Country = request.Country;
        }

        if (request.Interests is not null)
        {
            user.Interests = request.Interests.ToList();
        }

        if (request.Habits is not null)
        {
            user.Habits = request.Habits.ToList();
        }

        if (request.Personality is not null)
        {
            user.Personality = request.Personality.ToList();
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await _dbContext.SaveChangesAsync();

        return ToResponse(user);
    }

    private static UserResponse ToResponse(User user) => new(
        user.Id,
        user.Email ?? string.Empty,
        user.DisplayName,
        user.Role.ToString(),
        user.AvatarUrl,
        user.City,
        user.Country,
        user.OnboardingComplete);
}

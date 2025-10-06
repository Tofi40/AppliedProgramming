using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomieMatch.Api.Extensions;
using RoomieMatch.Api.Models;
using RoomieMatch.Application.Matching;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MatchingEngine _matchingEngine;

    public RoomsController(ApplicationDbContext dbContext, MatchingEngine matchingEngine)
    {
        _dbContext = dbContext;
        _matchingEngine = matchingEngine;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetRooms([FromQuery] Guid? seekerId = null)
    {
        var rooms = await _dbContext.Rooms.Include(r => r.Owner).Where(r => r.IsPublished).ToListAsync();
        var responses = new List<RoomResponse>();
        foreach (var room in rooms)
        {
            double? score = null;
            string[]? rationale = null;
            if (seekerId.HasValue)
            {
                var result = await _matchingEngine.ScoreSeekerToRoom(seekerId.Value, room.Id);
                score = result.Score;
                rationale = result.Rationale;
            }
            responses.Add(ToResponse(room, score, rationale));
        }
        return responses;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoom(Guid id, [FromQuery] Guid? seekerId = null)
    {
        var room = await _dbContext.Rooms.Include(r => r.Owner).FirstOrDefaultAsync(r => r.Id == id);
        if (room == null)
        {
            return NotFound();
        }

        double? score = null;
        string[]? rationale = null;
        if (seekerId.HasValue)
        {
            var result = await _matchingEngine.ScoreSeekerToRoom(seekerId.Value, room.Id);
            score = result.Score;
            rationale = result.Rationale;
        }
        return ToResponse(room, score, rationale);
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpPost]
    public async Task<ActionResult<RoomResponse>> CreateRoom(CreateRoomRequest request)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty)
        {
            return Unauthorized();
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = request.Title,
            Description = request.Description,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            Country = request.Country,
            AvailableFrom = request.AvailableFrom,
            MinTermMonths = request.MinTermMonths,
            PricePerMonthCents = request.PricePerMonthCents,
            BillsIncluded = request.BillsIncluded,
            Amenities = request.Amenities.ToList(),
            HouseRules = request.HouseRules.ToList(),
            Photos = request.Photos.ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPublished = false
        };

        _dbContext.Rooms.Add(room);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, ToResponse(room, null, null));
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> UpdateRoom(Guid id, UpdateRoomRequest request)
    {
        var ownerId = User.GetUserId();
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id && r.OwnerId == ownerId);
        if (room == null)
        {
            return NotFound();
        }

        room.Title = request.Title;
        room.Description = request.Description;
        room.AddressLine1 = request.AddressLine1;
        room.AddressLine2 = request.AddressLine2;
        room.City = request.City;
        room.Country = request.Country;
        room.AvailableFrom = request.AvailableFrom;
        room.MinTermMonths = request.MinTermMonths;
        room.PricePerMonthCents = request.PricePerMonthCents;
        room.BillsIncluded = request.BillsIncluded;
        room.Amenities = request.Amenities.ToList();
        room.HouseRules = request.HouseRules.ToList();
        room.Photos = request.Photos.ToList();
        if (request.IsPublished.HasValue)
        {
            room.IsPublished = request.IsPublished.Value;
        }
        room.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return ToResponse(room, null, null);
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var ownerId = User.GetUserId();
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id && r.OwnerId == ownerId);
        if (room == null)
        {
            return NotFound();
        }

        _dbContext.Rooms.Remove(room);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static RoomResponse ToResponse(Room room, double? score, string[]? rationale) => new(
        room.Id,
        room.Title,
        room.Description,
        room.City,
        room.Country,
        room.PricePerMonthCents,
        room.BillsIncluded,
        room.IsPublished,
        score,
        rationale);
}

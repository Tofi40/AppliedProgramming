using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomieMatch.Api.Extensions;
using RoomieMatch.Api.Models;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public RequestsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<ActionResult<BookingRequestResponse>> Create(CreateBookingRequest request)
    {
        var seekerId = User.GetUserId();
        if (seekerId == Guid.Empty)
        {
            return Unauthorized();
        }

        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null)
        {
            return NotFound("Room not found");
        }

        var booking = new BookingRequest
        {
            Id = Guid.NewGuid(),
            RoomId = request.RoomId,
            SeekerId = seekerId,
            Status = BookingRequestStatus.Pending,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.BookingRequests.Add(booking);
        await _dbContext.SaveChangesAsync();
        return ToResponse(booking);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingRequestResponse>>> List()
    {
        var userId = User.GetUserId();
        if (User.IsInRole(nameof(UserRole.Owner)))
        {
            var requests = await _dbContext.BookingRequests
                .Where(r => r.Room.OwnerId == userId)
                .Include(r => r.Room)
                .ToListAsync();
            return requests.Select(ToResponse).ToList();
        }
        else
        {
            var requests = await _dbContext.BookingRequests
                .Where(r => r.SeekerId == userId)
                .ToListAsync();
            return requests.Select(ToResponse).ToList();
        }
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<ActionResult<BookingRequestResponse>> Update(Guid id, UpdateBookingRequest request)
    {
        var ownerId = User.GetUserId();
        var booking = await _dbContext.BookingRequests
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id && b.Room.OwnerId == ownerId);
        if (booking == null)
        {
            return NotFound();
        }

        booking.Status = request.Status;
        booking.Note = request.Note;
        booking.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return ToResponse(booking);
    }

    private static BookingRequestResponse ToResponse(BookingRequest request) => new(
        request.Id,
        request.RoomId,
        request.SeekerId,
        request.Status,
        request.Note,
        request.CreatedAt,
        request.UpdatedAt);
}

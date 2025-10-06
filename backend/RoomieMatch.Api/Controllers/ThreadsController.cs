using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RoomieMatch.Api.Extensions;
using RoomieMatch.Api.Models;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;
using RoomieMatch.SignalR.Hubs;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/threads")]
[Authorize]
public class ThreadsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHubContext<MessagingHub> _hubContext;

    public ThreadsController(ApplicationDbContext dbContext, IHubContext<MessagingHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageThreadResponse>>> List()
    {
        var userId = User.GetUserId();
        var threads = await _dbContext.MessageThreads
            .Where(t => t.OwnerId == userId || t.SeekerId == userId)
            .ToListAsync();
        return threads.Select(ToResponse).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<MessageThreadResponse>> Create(CreateThreadRequest request)
    {
        var thread = new MessageThread
        {
            Id = Guid.NewGuid(),
            RoomId = request.RoomId,
            OwnerId = request.OwnerId,
            SeekerId = request.SeekerId,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };
        _dbContext.MessageThreads.Add(thread);
        await _dbContext.SaveChangesAsync();
        return ToResponse(thread);
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages(Guid id)
    {
        var userId = User.GetUserId();
        var thread = await _dbContext.MessageThreads.FirstOrDefaultAsync(t => t.Id == id && (t.OwnerId == userId || t.SeekerId == userId));
        if (thread == null)
        {
            return NotFound();
        }

        var messages = await _dbContext.Messages.Where(m => m.ThreadId == id).OrderBy(m => m.CreatedAt).ToListAsync();
        return messages.Select(ToResponse).ToList();
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<MessageResponse>> SendMessage(Guid id, SendMessageRequest request)
    {
        var userId = User.GetUserId();
        var thread = await _dbContext.MessageThreads.FirstOrDefaultAsync(t => t.Id == id && (t.OwnerId == userId || t.SeekerId == userId));
        if (thread == null)
        {
            return NotFound();
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ThreadId = id,
            SenderId = userId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Messages.Add(message);
        thread.LastMessageAt = message.CreatedAt;
        await _dbContext.SaveChangesAsync();
        await _hubContext.Clients.Group(id.ToString()).SendAsync("message", ToResponse(message));
        return ToResponse(message);
    }

    private static MessageThreadResponse ToResponse(MessageThread thread) => new(
        thread.Id,
        thread.RoomId,
        thread.OwnerId,
        thread.SeekerId,
        thread.CreatedAt,
        thread.LastMessageAt);

    private static MessageResponse ToResponse(Message message) => new(
        message.Id,
        message.ThreadId,
        message.SenderId,
        message.Content,
        message.CreatedAt);
}

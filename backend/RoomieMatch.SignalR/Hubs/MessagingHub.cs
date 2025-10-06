using Microsoft.AspNetCore.SignalR;

namespace RoomieMatch.SignalR.Hubs;

public class MessagingHub : Hub
{
    public async Task JoinThread(string threadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, threadId);
    }

    public async Task LeaveThread(string threadId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, threadId);
    }

    public async Task SendMessage(string threadId, string message)
    {
        await Clients.Group(threadId).SendAsync("message", new { ThreadId = threadId, Message = message });
    }
}

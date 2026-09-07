using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Services;
using MyAPI.Models;
using System.Security.Claims;

namespace MyAPI.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _context;
    private readonly ChatService _chatService;

    public ChatHub(AppDbContext context, ChatService chatService)
    {
        _context = context;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var user = await GetCurrentUser();
        if (user is null)
        {
            Context.Abort();
            return;
        }

        if (user.Role == "Admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
        }
        else
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(user.Id));
        }

        await base.OnConnectedAsync();
    }

    public async Task OpenConversation(int conversationUserId, int? previousConversationUserId = null)
    {
        var user = await GetCurrentUser();
        if (user is null || user.Role != "Admin")
        {
            throw new HubException("Only admins can open another user's conversation.");
        }

        var target = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == conversationUserId && item.Role != "Admin" && !item.IsDeleted);

        if (target is null)
        {
            throw new HubException("Conversation not found.");
        }

        if (previousConversationUserId is not null && previousConversationUserId != conversationUserId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(previousConversationUserId.Value));
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationUserId));
    }

    public async Task SendMessage(string message, int? conversationUserId = null)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > 4000)
        {
            throw new HubException("Message must contain 1-4000 characters.");
        }

        var sender = await GetCurrentUser();
        if (sender is null)
        {
            throw new HubException("You must be logged in to send a message.");
        }

        var targetConversationUserId = conversationUserId ?? sender.Id;
        if (sender.Role != "Admin")
        {
            targetConversationUserId = sender.Id;
        }
        else
        {
            var target = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == targetConversationUserId && item.Role != "Admin" && !item.IsDeleted);
            if (target is null)
            {
                throw new HubException("Conversation not found.");
            }
        }

        var chatMessage = await _chatService.AddMessage(targetConversationUserId, sender.Id, message);
        var dto = new
        {
            id = chatMessage.Id,
            conversationUserId = chatMessage.ConversationUserId,
            senderUsername = chatMessage.SenderUser.Username,
            message = chatMessage.Message,
            sentAt = chatMessage.SentAt
        };

        await Clients.Group(ConversationGroup(targetConversationUserId)).SendAsync("ReceiveMessage", dto);
        await Clients.Group("admins").SendAsync("ConversationUpdated", targetConversationUserId);
    }

    private string ConversationGroup(int userId) => $"conversation:{userId}";

    private async Task<User?> GetCurrentUser()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userId, out var parsedUserId))
        {
            return await _context.Users.FirstOrDefaultAsync(user =>
                user.Id == parsedUserId && !user.IsDeleted);
        }

        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value
            ?? Context.User?.FindFirst("name")?.Value
            ?? Context.User?.Identity?.Name;

        return string.IsNullOrWhiteSpace(username)
            ? null
            : await _context.Users.FirstOrDefaultAsync(user => user.Username == username && !user.IsDeleted);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Services;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ChatService _chatService;

    public ChatController(AppDbContext context, ChatService chatService)
    {
        _context = context;
        _chatService = chatService;
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IEnumerable<object>>> GetMessages([FromQuery] int? conversationUserId = null)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser is null)
        {
            return Unauthorized();
        }

        var targetUserId = currentUser.Role == "Admin" ? conversationUserId : currentUser.Id;
        if (targetUserId is null)
        {
            return BadRequest("conversationUserId is required for admins.");
        }

        if (currentUser.Role == "Admin")
        {
            var target = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == targetUserId && user.Role != "Admin" && !user.IsDeleted);
            if (target is null)
            {
                return NotFound();
            }
        }
        else if (targetUserId != currentUser.Id)
        {
            return Forbid();
        }

        var messages = await _chatService.GetMessages(targetUserId.Value);
        return Ok(messages.Select(message => new
        {
            id = message.Id,
            conversationUserId = message.ConversationUserId,
            senderUsername = message.SenderUser.Username,
            message = message.Message,
            sentAt = message.SentAt
        }));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<object>>> GetConversations()
    {
        var users = await _chatService.GetConversations();
        var lastMessages = await _context.ChatMessages
            .AsNoTracking()
            .GroupBy(message => message.ConversationUserId)
            .Select(group => group.OrderByDescending(message => message.SentAt).First())
            .ToDictionaryAsync(message => message.ConversationUserId);

        return Ok(users.Select(user => new
        {
            id = user.Id,
            username = user.Username,
            lastMessage = lastMessages.TryGetValue(user.Id, out var message)
                ? new { text = message.Message, sentAt = message.SentAt }
                : null
        }));
    }

    private Task<User?> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userId, out var parsedUserId))
        {
            return _context.Users.FirstOrDefaultAsync(user =>
                user.Id == parsedUserId && !user.IsDeleted);
        }

        var username = User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue("name")
            ?? User.Identity?.Name;

        return string.IsNullOrWhiteSpace(username)
            ? Task.FromResult<User?>(null)
            : _context.Users.FirstOrDefaultAsync(user =>
                user.Username == username && !user.IsDeleted);
    }
}

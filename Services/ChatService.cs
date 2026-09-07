using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;

namespace MyAPI.Services;

public class ChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<ChatMessage>> GetMessages(int conversationUserId) =>
        _context.ChatMessages
            .AsNoTracking()
            .Include(message => message.SenderUser)
            .Where(message => message.ConversationUserId == conversationUserId)
            .OrderBy(message => message.SentAt)
            .ToListAsync();

    public Task<List<User>> GetConversations() =>
        _context.Users
            .AsNoTracking()
            .Where(user => user.Role != "Admin" && !user.IsDeleted)
            .OrderBy(user => user.Username)
            .ToListAsync();

    public async Task<ChatMessage> AddMessage(int conversationUserId, int senderUserId, string message)
    {
        var chatMessage = new ChatMessage
        {
            ConversationUserId = conversationUserId,
            SenderUserId = senderUserId,
            Message = message.Trim(),
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();
        return await _context.ChatMessages
            .Include(item => item.SenderUser)
            .SingleAsync(item => item.Id == chatMessage.Id);
    }
}

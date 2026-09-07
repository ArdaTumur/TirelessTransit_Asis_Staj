namespace MyAPI.Models;

public class ChatMessage
{
    public long Id { get; set; }

    // Every conversation belongs to one regular user. Admins reply into that same thread.
    public int ConversationUserId { get; set; }

    public User ConversationUser { get; set; } = null!;

    public int SenderUserId { get; set; }

    public User SenderUser { get; set; } = null!;

    public string Message { get; set; } = "";

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

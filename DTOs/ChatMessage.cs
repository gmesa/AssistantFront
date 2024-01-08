namespace AccountingAssistant.DTOs
{
    public record class ChatMessage(int Id, int SessionChatId, string Content, bool IsFromAssistant, DateTime CreatedAt) { }

}

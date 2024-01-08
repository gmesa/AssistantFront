namespace AccountingAssistant.DTOs
{
    public record CreateChatMessage(int SessionChatId, string Content, bool IsFromAssistant)
    {
    }
}

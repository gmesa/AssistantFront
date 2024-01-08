using AccountingAssistant.DTOs;

namespace AccountingAssistant.Services
{
    public interface IAssistantService
    {
        Task<string> GetAnswerForUserQuestion(string question);

        Task<List<SessionChat>> GetSessionsChat(int userId, int count);

        Task<SessionChat> AddSessionChat(CreateSessionChat chat);

        Task<List<ChatMessage>> GetChatMessages(int chatId, int count);

        Task<ChatMessage> AddChatMessage(CreateChatMessage message);
    }


}

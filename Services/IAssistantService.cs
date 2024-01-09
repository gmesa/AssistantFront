using AccountingAssistant.DTOs;

namespace AccountingAssistant.Services
{
    public interface IAssistantService
    {
        /// <summary>
        /// Retrieves the answer for a user's question
        /// </summary>
        /// <param name="question">The question asked by the user.</param>
        /// <param name="sessionChatId">The ID of the session chat.</param>
        /// <returns>A string representing the answer to the user's question.</returns>
        Task<string> GetAnswerForUserQuestion(int sessionChatId, string question);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<string> GetSummaryFromPdfFile(MultipartFormDataContent content);

        /// <summary>
        /// Retrieves a list of session chats for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="count">The number of chats to retrieve.</param>
        /// <returns>A list of the retrieve session chats.</returns>
        Task<List<SessionChat>> GetSessionsChat(int userId, int count);

        /// <summary>
        /// Adds a session chat for current user.
        /// </summary>
        /// <param name="chat">The session chat to be added.</param>
        /// <returns>The session chat added</returns>
        Task<SessionChat> AddSessionChat(CreateSessionChat chat);

        /// <summary>
        /// Retrieves a list of chat messages based on the specified session chat ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to retrieve messages from.</param>
        /// <param name="count">The number of messages to retrieve.</param>
        /// <returns>List of the retrieved chat messages</returns>
        Task<List<ChatMessage>> GetChatMessages(int chatId, int count);

        /// <summary>
        /// Adds a chat message to the current session chat.
        /// </summary>
        /// <param name="message">The chat message to be added.</param>
        /// <returns>The chat message added</returns>
        Task<ChatMessage> AddChatMessage(CreateChatMessage message);
       
    }


}

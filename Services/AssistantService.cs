using AccountingAssistant.Clients;
using AccountingAssistant.DTOs;

namespace AccountingAssistant.Services
{
    /// <summary>
    /// Assistant service
    /// </summary>
    public class AssistantService : IAssistantService
    {
        private readonly AssistantClient _assistantClient;

        /// <summary>
        /// Creates a new instance of <see cref="AssistantService"/>
        /// </summary>
        /// <param name="assistantClient"></param>
        public AssistantService(AssistantClient assistantClient)
        {
            _assistantClient = assistantClient;
        }

        #region AI

        /// <summary>
        /// Retrieves the answer for a user's question
        /// </summary>
        /// <param name="question">The question asked by the user.</param>
        /// <param name="sessionChatId">The ID of the chat message.</param>
        /// <returns>A string representing the answer to the user's question.</returns>
        public async Task<string> GetAnswerForUserQuestion(int sessionChatId, string question)
        {
            string response = await _assistantClient.GetAnswerForUserQuestion(sessionChatId, question);

            return response != null ? response : "There was a problem trying to comunicate with the assistant. Please try again";
        }

        /// <summary>
        /// Retrieves a summary from a PDF file.
        /// </summary>
        /// <param name="content">The multipart form data content containing the PDF file.</param>
        /// <returns>The summary extracted from the PDF file.</returns>
        public async Task<string> GetSummaryFromPdfFile(MultipartFormDataContent content)
        {
            string response = await _assistantClient.GetSummaryFromPdf(content);

            return response != null ? response : "There was a problem trying to comunicate with the assistant. Please try again";
        }

        #endregion

        #region Session chats

        /// <summary>
        /// Retrieves a list of chats for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="count">The number of chats to retrieve.</param>
        /// <returns>A list of the retrieve chats.</returns>
        public async Task<List<SessionChat>> GetSessionsChat(int userId, int count)
        {
            return await _assistantClient.GetSessionsChat(userId, count);

        }

        /// <summary>
        /// Adds a chat session to the chat for current user.
        /// </summary>
        /// <param name="chat">The chat session to be added.</param>
        /// <returns>The chat session added</returns>
        public async Task<SessionChat> AddSessionChat(CreateSessionChat chat)
        {
            return await _assistantClient.AddSessionsChat(chat);
        }

        #endregion

        #region Chat messages

        /// <summary>
        /// Retrieves a list of chat messages based on the specified chat ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to retrieve messages from.</param>
        /// <param name="count">The number of messages to retrieve.</param>
        /// <returns>List of the retrieved chat messages</returns>
        public async Task<List<ChatMessage>> GetChatMessages(int chatId, int count)
        {
            return await _assistantClient.GetChatMessages(chatId, count)
                ?? new List<ChatMessage>();
        }

        /// <summary>
        /// Adds a chat message to the current chat session.
        /// </summary>
        /// <param name="message">The chat message to be added.</param>
        /// <returns>The message added</returns>
        public async Task<ChatMessage> AddChatMessage(CreateChatMessage message)
        {
            return await _assistantClient.AddChatMessages(message);
        }       


        #endregion

    }
}

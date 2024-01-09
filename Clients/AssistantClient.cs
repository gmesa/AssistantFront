using AccountingAssistant.Configuration;
using AccountingAssistant.DTOs;
using AccountingAssistant.Extensions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AccountingAssistant.Clients
{
    /// <summary>
    /// Client to interact with the Assistant API
    /// </summary>
    public class AssistantClient
    {
        public HttpClient HttpClient { get; }
        private readonly ILogger<AssistantClient> _logger;
        private readonly JsonSerializerOptions _options;
        private readonly AssistantClientOptions _assistantClientOptions;

        /// <summary>
        /// Create an instance of the <see cref="AssistantClient"/>
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="configuration">Configuration</param>
        public AssistantClient(HttpClient client, IConfiguration configuration, ILogger<AssistantClient> logger)
        {
            _logger = logger;

            _assistantClientOptions = new AssistantClientOptions();
            configuration.GetSection(AssistantClientOptions.ConfigurationSectionName).Bind(_assistantClientOptions);

            string baseAddress = _assistantClientOptions.BaseUrl;
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(baseAddress);
            HttpClient.Timeout = new TimeSpan(0, 10, 0);

            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        }

        #region AI

        /// <summary>
        /// Retrieves the answer for a user's question
        /// </summary>
        /// <param name="question">The question asked by the user.</param>
        /// <param name="sessionChatId">The id of the session chat.</param>
        /// <returns>A string representing the answer to the user's question.</returns>
        public async Task<string> GetAnswerForUserQuestion(int sessionChatId, string question)
        {

            string url = $"v1/assistant/sessionChatId/{sessionChatId}/query/{question}";

            try
            {
                using (var response = await HttpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to comunicate with the assistant API");
                return null;
            }


        }

        public async Task<string> GetSummaryFromPdf(MultipartFormDataContent content) {

            string url = $"v1/assistant/documentSummary";

            try
            {
                
                CancellationTokenSource timeoutSource = new CancellationTokenSource(_assistantClientOptions.TimeOutForSummaryPdf);
                using (var response = await HttpClient.PostAsync(url, content, timeoutSource.Token))
                {
                    timeoutSource.Dispose();
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to comunicate with the assistant API");
                return null;
            }
        }

        #endregion

        #region Sessions chat

        /// <summary>
        /// Retrieves the session chats for the specific user.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SessionChat>> GetSessionsChat(int userId, int count)
        {

            string url = $"v1/sessionChat/user/{userId}?count={count}";

            try
            {
                using (var response = await HttpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var chats = JsonSerializer.Deserialize<List<SessionChat>>(content, _options);
                    return chats;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to get all chats for the specific user");
                return null;
            }


        }

        /// <summary>
        /// Add a new chat
        /// </summary>
        /// <param name="chat">The chat object to be added.</param>
        /// <returns></returns>
        public async Task<SessionChat> AddSessionsChat(CreateSessionChat chat)
        {

            string url = $"v1/sessionChat";
            try
            {
                using (var response = await HttpClient.PostAsync(url, chat.ToJson().AsContent()))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var chats = JsonSerializer.Deserialize<SessionChat>(content, _options);
                    return chats;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error adding a chat");
                return null;
            }

        }

        #endregion

        #region Chats messages

        /// <summary>
        /// Retrieves the chat messages for the specific chat.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChatMessage>> GetChatMessages(int chatId, int count)
        {

            string url = $"v1/chatMessage/{chatId}/messages?count={count}";

            try
            {
                using (var response = await HttpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var chatMessages = JsonSerializer.Deserialize<List<ChatMessage>>(content, _options);
                    return chatMessages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to get all chats for the specific user");
                return null;
            }


        }

        /// <summary>
        /// Add message to a chat
        /// </summary>
        /// <param name="message">Message object to be added</param>
        /// <returns></returns>
        public async Task<ChatMessage> AddChatMessages(CreateChatMessage message)
        {

            string url = $"v1/chatMessage/";

            try
            {
                using (var response = await HttpClient.PostAsync(url, message.ToJson().AsContent()))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var chats = JsonSerializer.Deserialize<ChatMessage>(content, _options);
                    return chats;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error adding a chat message");
                return null;
            }


        }

        #endregion

    }
}

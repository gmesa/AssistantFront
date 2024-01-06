using System.Text.Json;

namespace AccountingAssistant.Clients
{
    public class AssistantClient
    {
        public HttpClient HttpClient { get; }

        private readonly JsonSerializerOptions jsonSerializerOptions;
        public AssistantClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            string baseAddress = configuration["AssistantClient:BaseUrl"]!;
            HttpClient = httpClientFactory.CreateClient();
            HttpClient.BaseAddress = new Uri(baseAddress);
            HttpClient.Timeout = new TimeSpan(0, 0, 30);

            jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<string> GetAnswerForUserQuestion(string question) {

            string url = $"accounting/query/{question}";
            using (var response = await HttpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
                
            }
        }
    }
}

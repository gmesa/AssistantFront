using AccountingAssistant.Clients;

namespace AccountingAssistant.Services
{
    public class AssistantService : IAssistantService
    {
        private readonly AssistantClient _assistantClient;
        public AssistantService(AssistantClient assistantClient)
        {
            _assistantClient = assistantClient;
        }

        public async Task<string> GetAnswerForUserQuestion(string question)
        {
            try
            {
                string response = await _assistantClient.GetAnswerForUserQuestion(question);
                return response;
            }
            catch (Exception ex)
            {
                return "There was an error while trying to communicate with the assistant";
            }

        }
    }
}

namespace AccountingAssistant.Services
{
    public interface IAssistantService
    {
        Task<string> GetAnswerForUserQuestion(string question);
    }
}

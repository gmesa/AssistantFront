using AccountingAssistant.DTOs;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;


namespace AccountingAssistant.Shared
{
    public partial class ChatMessages
    {
        [Inject]
        public IAssistantService AssistantService { get; set; }

        [Parameter]
        public List<ChatMessage> Messages { get; set; }

        private string Question { get; set; }

        [Parameter]
        public EventCallback<string> OnMessageSent { get; set; }

        private async Task SendMessage()
        {
            await OnMessageSent.InvokeAsync(Question);
            Question = string.Empty;

        }

    }
}
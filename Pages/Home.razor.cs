using AccountingAssistant.Clients;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;

namespace AccountingAssistant.Pages
{
    public partial class Home
    {
        [Inject]
        public IAssistantService AssistantService { get; set; }

        private string response = String.Empty;

        protected async override Task OnInitializedAsync()
        {
           response = await AssistantService.GetAnswerForUserQuestion("Hello");
            
        }

    }
}
using AccountingAssistant.Clients;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AccountingAssistant.Pages
{
    public partial class Home
    {
       private ErrorBoundary errorBoundary { get; set; }       
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
    }
}
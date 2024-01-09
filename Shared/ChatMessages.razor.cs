using AccountingAssistant.DTOs;
using AccountingAssistant.Extensions;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;


namespace AccountingAssistant.Shared
{
    public partial class ChatMessages
    {
        [Inject]
        public IAssistantService AssistantService { get; set; }

        [Inject]
        public ILogger<ChatMessages> Logger { get; set; }

        [Parameter]
        public List<ChatMessage> Messages { get; set; }

        [Parameter]
        public EventCallback<string> OnMessageSent { get; set; }

        [Parameter]
        public EventCallback<MultipartFormDataContent> OnFileUpload { get; set; }

        private string Question { get; set; }

        private bool VisiblePdfForm { get; set; } = false;

        [Parameter]
        public bool ProcessingFile { get; set; } = false;

        private long MaxAllowedFileSize { get; } = long.MaxValue;

        private async Task TooglePdfFormVisible()
        {
            VisiblePdfForm = !VisiblePdfForm;
        }

        private async Task SendMessage()
        {
            string cloneQuestion = new string(Question.ToCharArray());
            Question = string.Empty;
            await OnMessageSent.InvokeAsync(cloneQuestion);
            


        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {

            try
            {
                using var content = new MultipartFormDataContent();
                var file = e.File;
                var fileContent = new StreamContent(file.OpenReadStream(MaxAllowedFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(
                    content: fileContent,
                    name: "\"PdfFile\"",
                    fileName: file.Name
                             );
                await OnFileUpload.InvokeAsync(content);
            }
            catch (Exception ex)
            {

                Logger.LogInformation(
                       "{FileName} not uploaded (Err: 6): {Message}",
                       e.File.Name, ex.Message);


            }

        }

    }
}

using AccountingAssistant.Configuration;
using AccountingAssistant.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace AccountingAssistant.Shared
{
    public partial class Chats
    {
        [Inject]
        public IOptions<ChatOptions> chatOptions { get; set; }

        [Parameter]
        public EventCallback<SessionChat> ChangedCurrentChat { get; set; }

        [Parameter]
        public List<SessionChat> chats { get; set; }     

        private void AddNewChat()
        {
            if (chats == null || chats.Count == 0 || chats[0].Id > 0)
            {
                var newSessionChat = new SessionChat(-1, chatOptions.Value.DefaultUserId, "Write a question", DateTime.Now);
                chats.Insert(0, newSessionChat);
                SetCurrentChat(newSessionChat);
            }       
        }

        private void SetCurrentChat(SessionChat chat)
        {
            ChangedCurrentChat.InvokeAsync(chat);
        }
    }

}
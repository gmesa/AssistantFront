using AccountingAssistant.Configuration;
using AccountingAssistant.DTOs;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System;


namespace AccountingAssistant.Shared
{
    public partial class Assistant
    {
        [Inject]
        public IAssistantService AssistantService { get; set; }

        [Inject]
        public IOptions<ChatOptions> chatOptions { get; set; }

        [Inject]
        public ILogger<Assistant> logger { get; set; }

        public List<ChatMessage> messages { get; set; }

        public List<SessionChat> chats { get; set; }

        public SessionChat CurrentChat { get; set; }

        protected async override Task OnInitializedAsync()
        {
            chats = await AssistantService.GetSessionsChat(chatOptions.Value.DefaultUserId, chatOptions.Value.ChatMessageQuantity)
                ?? new List<SessionChat>();
        }

        private async Task ChangeCurrentChat(SessionChat chat)
        {
            CurrentChat = chat;
            var messageList = await AssistantService.GetChatMessages(CurrentChat.Id, chatOptions.Value.ChatsQuantity);               
            messageList.Reverse();
            messages = messageList;
        }

        /// <summary>
        /// Proccess the user question.
        /// If there is no session chat selected or the selected session chat is new, create a new session chat.
        ///    The title of the session chat will be the first question. 
        /// The question is sent to the assistant.
        /// The question and the assistant response are added to the list of chat messages for the current session chat.
        /// </summary>
        /// <param name="question">The question to send to the assistant.</param>
        /// <returns></returns>
        private async Task SendQuestion(string question)
        {
            try
            {
                //If there is no chat selected or is a new one, it will be created a new session chat
                if (CurrentChat == null || CurrentChat.Id < 0)
                {
                    var newChat = new CreateSessionChat(chatOptions.Value.DefaultUserId, question);
                    var chatAdded = await AssistantService.AddSessionChat(newChat);

                    if (CurrentChat == null)
                        chats.Insert(0, chatAdded);
                    else
                        chats[0] = chatAdded;

                    CurrentChat = chatAdded;

                    messages = new List<ChatMessage>();

                }

                // Add the question to the list of messages for the current session chat
                var chatMessage = new CreateChatMessage(CurrentChat.Id, question, false);
                var chatMessageAdded = await AssistantService.AddChatMessage(chatMessage);
                messages.Add(chatMessageAdded);

                // Send the question to the assistant
                string response = await AssistantService.GetAnswerForUserQuestion(question);

                // Add the assistant response to the list of messages for the current session chat
                chatMessage = new CreateChatMessage(CurrentChat.Id, response, true);
                chatMessageAdded = await AssistantService.AddChatMessage(chatMessage);
                messages.Add(chatMessageAdded);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error sending question to the assistant");
                messages ??= new List<ChatMessage>();
                messages.Add(new ChatMessage(-1,
                                              -1,
                                              "There was a problem trying to comunicate with the assistant. Please try again",
                                              true,
                                              DateTime.Now));

            }


        }


    }
}
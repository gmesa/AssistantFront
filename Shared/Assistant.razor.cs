using AccountingAssistant.Configuration;
using AccountingAssistant.DTOs;
using AccountingAssistant.Extensions;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Runtime.ConstrainedExecution;


namespace AccountingAssistant.Shared
{
    public partial class Assistant
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IAssistantService AssistantService { get; set; }

        [Inject]
        public IOptions<ChatOptions> chatOptions { get; set; }

        [Inject]
        public ILogger<Assistant> logger { get; set; }

        public List<ChatMessage> messages { get; set; }

        public List<SessionChat> chats { get; set; }

        public SessionChat CurrentChat { get; set; }

        public bool ProcessingFile { get; set; } = false;

        private IJSObjectReference _jsModule;

        protected async override Task OnInitializedAsync()
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/assistant.js");

            chats = await AssistantService.GetSessionsChat(chatOptions.Value.DefaultUserId, chatOptions.Value.ChatMessageQuantity)
                ?? new List<SessionChat>();

        }

        /// <summary>
        /// Changes the current chat to the specified session chat. Load all the latest chat messages for the session chat.
        /// </summary>
        /// <param name="chat">The current chat </param>
        /// <returns></returns>
        private async Task ChangeCurrentChat(SessionChat chat)
        {
            CurrentChat = chat;
            var messageList = await AssistantService.GetChatMessages(CurrentChat.Id, chatOptions.Value.ChatsQuantity);
            messages = new List<ChatMessage>();

            for (int i = messageList.Count - 1; i >= 0; i--)
            {
                var message = messageList[i];
                message = message with { Content = message.Content.ConvertToHtmlParagraph() };
                messages.Add(message);
            }

            await ChatContainerScrollToBottom();


        }

        /// <summary>
        /// Proccess the user question.
        /// If there is no session chat selected or the selected session chat is new, create a new session chat.
        ///    The title of the session chat will be the question. 
        /// The question is sent to the assistant.
        /// The question and the assistant response are added to the list of chat messages for the current session chat.
        /// </summary>
        /// <param name="question">The question to send to the assistant.</param>
        /// <returns>The response from the assistant</returns>
        private async Task SendQuestion(string question)
        {
            try
            {
                Func<object, int?, Task<string>> request =
                    (query, sessionChatId) => AssistantService.GetAnswerForUserQuestion(sessionChatId.Value, (string)query);

                await SendRequestToAssistant(question, request);
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


        /// <summary>
        /// Proccess the pdf document.
        /// If there is no session chat selected or the selected session chat is new, create a new session chat.
        ///    The title of the session chat will be the question: "Write a summary for this pdf document".
        /// The pdf document is sent to the assistant.
        /// The question and the assistant response are added to the list of chat messages for the current session chat.
        /// </summary>
        /// <param name="content">The pdf document to send to the assistant.</param>
        /// <returns></returns>
        private async Task OnFileUpload(MultipartFormDataContent content)
        {
            try
            {
                ProcessingFile = true;
                Func<object, int?, Task<string>> request =
                    (file, _) => AssistantService.GetSummaryFromPdfFile((MultipartFormDataContent)file);

                await SendRequestToAssistant(
                    $"Please summarize {content.ElementAt(0).Headers.ContentDisposition.FileName} document: ", 
                    request, 
                    content);
                ProcessingFile = false;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error sending the pdf document to the assistant");
                messages ??= new List<ChatMessage>();
                messages.Add(new ChatMessage(-1,
                                              -1,
                                              "There was a problem trying to comunicate with the assistant. Please try again",
                                              true,
                                              DateTime.Now));

            }

        }


        /// <summary>
        /// Send a request to the assistant.
        /// </summary>
        /// <param name="question">A user question to the assitant</param>
        /// <param name="request">The specific request to the assitant </param>
        /// <param name="content">Optional: The pdf document to send to the assistant</param>
        /// <returns></returns>
        private async Task SendRequestToAssistant(string question, Func<object, int?, Task<string>> request, MultipartFormDataContent content = null)
        {

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
            await ChatContainerScrollToBottom();

            string response;
            // Send the question or pdf to the assistant
            if (content != null)
            {
                response = await request.Invoke(content, null);
            }
            else
            {
                response = await request.Invoke(question, chatMessageAdded.SessionChatId);
            }


            // Add the assistant response to the list of messages for the current session chat
            chatMessage = new CreateChatMessage(CurrentChat.Id, response, true);
            chatMessageAdded = await AssistantService.AddChatMessage(chatMessage);
            chatMessageAdded = chatMessageAdded with { Content = chatMessageAdded.Content.ConvertToHtmlParagraph() };
            messages.Add(chatMessageAdded);
            await ChatContainerScrollToBottom();

        }


        /// <summary>
        /// Call StateHasChanged to refresh the UI and and scroll to the bottom
        /// </summary>
        /// <returns></returns>
        private async Task ChatContainerScrollToBottom()
        {
            StateHasChanged();
            await _jsModule.InvokeVoidAsync("scrollToBottom", "messageContainer");
        }
    }
}
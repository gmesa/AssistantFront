namespace AccountingAssistant.Configuration
{
    public class ChatOptions
    {
        public const string ConfigurationSectionName = "ChatOptions";
        public int ChatsQuantity { get; set; }

        public int DefaultUserId { get; set; }

        public int ChatMessageQuantity { get; set; }
    }
}

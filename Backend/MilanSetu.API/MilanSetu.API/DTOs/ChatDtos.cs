using System;
using System.Collections.Generic;

namespace MilanSetu.API.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; }
    }

    public class ConversationSummaryDto
    {
        public int UserId { get; set; }
        public string ProfileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; } = true;
        public bool IsVerified { get; set; } = true;
    }

    public class SendChatMessageRequest
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}

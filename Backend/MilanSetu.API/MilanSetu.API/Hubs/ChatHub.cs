using Microsoft.AspNetCore.SignalR;
using MilanSetu.API.Data;
using MilanSetu.API.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilanSetu.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int receiverId, string content)
        {
            var senderIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int senderId = int.TryParse(senderIdStr, out var id) ? id : 1;

            var msg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            var messagePayload = new
            {
                id = msg.Id,
                senderId = senderId,
                receiverId = receiverId,
                content = content,
                sentAt = msg.SentAt,
                isRead = false
            };

            // Send to Receiver group
            await Clients.Group($"User_{receiverId}").SendAsync("ReceiveMessage", messagePayload);

            // Echo back to Sender connection
            await Clients.Caller.SendAsync("MessageSentConfirmation", messagePayload);
        }

        public async Task SendTypingIndicator(int receiverId, bool isTyping)
        {
            var senderIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int senderId = int.TryParse(senderIdStr, out var id) ? id : 1;

            await Clients.Group($"User_{receiverId}").SendAsync("UserTyping", new { senderId, isTyping });
        }
    }
}

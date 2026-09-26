using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var id))
            {
                return id;
            }
            return 1;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetCurrentUserId();

            // Fetch connected partners and recent message contacts
            var connectedUsers = await _context.UserInterests
                .Include(i => i.SenderUser).ThenInclude(u => u.UserProfiles)
                .Include(i => i.ReceiverUser).ThenInclude(u => u.UserProfiles)
                .Where(i => (i.SenderUserId == userId || i.ReceiverUserId == userId) && i.Status == "Accepted")
                .ToListAsync();

            var conversations = new List<ConversationSummaryDto>();

            foreach (var conn in connectedUsers)
            {
                var other = conn.SenderUserId == userId ? conn.ReceiverUser : conn.SenderUser;
                var profile = other.UserProfiles.FirstOrDefault();

                var lastMsg = await _context.ChatMessages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == other.Id) || (m.SenderId == other.Id && m.ReceiverId == userId))
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefaultAsync();

                var unread = await _context.ChatMessages
                    .CountAsync(m => m.SenderId == other.Id && m.ReceiverId == userId && !m.IsRead);

                conversations.Add(new ConversationSummaryDto
                {
                    UserId = other.Id,
                    ProfileId = $"MS-{100 + other.Id}",
                    Name = other.Name,
                    ImageUrl = other.ProfilePhotoUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                    Location = other.Location,
                    Profession = profile?.Occupation ?? "Software Professional",
                    LastMessage = lastMsg?.Content ?? "Interest Accepted! Say Hello 👋",
                    LastMessageTime = lastMsg?.SentAt ?? conn.RespondedAt ?? DateTime.UtcNow,
                    UnreadCount = unread,
                    IsOnline = true,
                    IsVerified = other.IsVerified
                });
            }

            if (conversations.Count == 0)
            {
                conversations.AddRange(GetMockConversations());
            }

            return Ok(conversations.OrderByDescending(c => c.LastMessageTime));
        }

        [HttpGet("messages/{otherUserId}")]
        public async Task<IActionResult> GetMessages(int otherUserId)
        {
            var userId = GetCurrentUserId();

            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) || (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Mark unread as read
            var unread = messages.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();
            if (unread.Any())
            {
                foreach (var u in unread)
                {
                    u.IsRead = true;
                    u.ReadAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }

            var dtos = messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                IsRead = m.IsRead,
                SentAt = m.SentAt,
                IsMine = m.SenderId == userId
            }).ToList();

            if (dtos.Count == 0)
            {
                dtos.AddRange(GetMockMessages(otherUserId));
            }

            return Ok(dtos);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest req)
        {
            var userId = GetCurrentUserId();

            var msg = new ChatMessage
            {
                SenderId = userId,
                ReceiverId = req.ReceiverId,
                Content = req.Content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            return Ok(new ChatMessageDto
            {
                Id = msg.Id,
                SenderId = msg.SenderId,
                ReceiverId = msg.ReceiverId,
                Content = msg.Content,
                IsRead = false,
                SentAt = msg.SentAt,
                IsMine = true
            });
        }

        private static List<ConversationSummaryDto> GetMockConversations()
        {
            return new List<ConversationSummaryDto>
            {
                new ConversationSummaryDto
                {
                    UserId = 106,
                    ProfileId = "MS-106",
                    Name = "Karthik Ramanathan",
                    ImageUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80",
                    Location = "Chennai / Bengaluru",
                    Profession = "Engineering Manager at Google",
                    LastMessage = "Namaste! Are you free for a quick call this Sunday?",
                    LastMessageTime = DateTime.UtcNow.AddMinutes(-15),
                    UnreadCount = 1,
                    IsOnline = true,
                    IsVerified = true
                },
                new ConversationSummaryDto
                {
                    UserId = 203,
                    ProfileId = "MS-203",
                    Name = "Pooja Banerjee",
                    ImageUrl = "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
                    Location = "Kolkata / Gurugram",
                    Profession = "Finance Manager at Deloitte",
                    LastMessage = "Thank you so much! I really enjoyed reading about your hobbies.",
                    LastMessageTime = DateTime.UtcNow.AddHours(-2),
                    UnreadCount = 0,
                    IsOnline = false,
                    IsVerified = true
                },
                new ConversationSummaryDto
                {
                    UserId = 201,
                    ProfileId = "MS-201",
                    Name = "Ananya Sharma",
                    ImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                    Location = "Bengaluru, Karnataka",
                    Profession = "Senior Software Engineer at Microsoft",
                    LastMessage = "Interest Accepted! Let's get to know each other.",
                    LastMessageTime = DateTime.UtcNow.AddDays(-1),
                    UnreadCount = 0,
                    IsOnline = true,
                    IsVerified = true
                }
            };
        }

        private static List<ChatMessageDto> GetMockMessages(int otherUserId)
        {
            return new List<ChatMessageDto>
            {
                new ChatMessageDto
                {
                    Id = 1,
                    SenderId = otherUserId,
                    ReceiverId = 1,
                    Content = "Namaste! Thank you for accepting my interest on MilanSetu.",
                    IsRead = true,
                    SentAt = DateTime.UtcNow.AddHours(-3),
                    IsMine = false
                },
                new ChatMessageDto
                {
                    Id = 2,
                    SenderId = 1,
                    ReceiverId = otherUserId,
                    Content = "Hello! Very happy to connect with you. I found our partner preferences and family background strongly aligned.",
                    IsRead = true,
                    SentAt = DateTime.UtcNow.AddHours(-2).AddMinutes(40),
                    IsMine = true
                },
                new ChatMessageDto
                {
                    Id = 3,
                    SenderId = otherUserId,
                    ReceiverId = 1,
                    Content = "That's wonderful! Are you free for a quick voice call this weekend to introduce each other?",
                    IsRead = false,
                    SentAt = DateTime.UtcNow.AddMinutes(-15),
                    IsMine = false
                }
            };
        }
    }
}

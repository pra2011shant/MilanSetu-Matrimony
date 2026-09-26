using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
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
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
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

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetCurrentUserId();

            var list = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToListAsync();

            if (list.Count == 0)
            {
                list = GetMockNotifications(userId);
            }

            return Ok(list);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        private static List<Notification> GetMockNotifications(int userId)
        {
            return new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    UserId = userId,
                    Type = "Interest",
                    Title = "New Express Interest ❤️",
                    Message = "Rahul Deshmukh sent you an Express Interest invitation.",
                    ActionUrl = "/interests",
                    AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new Notification
                {
                    Id = 2,
                    UserId = userId,
                    Type = "ProfileView",
                    Title = "Profile Visitor 👀",
                    Message = "Dr. Simran Kaur Gill viewed your profile today.",
                    ActionUrl = "/matches",
                    AvatarUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new Notification
                {
                    Id = 3,
                    UserId = userId,
                    Type = "Message",
                    Title = "New Chat Message 💬",
                    Message = "Karthik Ramanathan: 'Namaste! Are you free for a call this Sunday?'",
                    ActionUrl = "/chat",
                    AvatarUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                },
                new Notification
                {
                    Id = 4,
                    UserId = userId,
                    Type = "Verification",
                    Title = "Profile Verification ✅",
                    Message = "Congratulations! Your email, mobile number, and profile have been 100% verified.",
                    ActionUrl = "/my-profile",
                    AvatarUrl = null,
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }
    }
}

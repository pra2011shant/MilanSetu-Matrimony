using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.Models;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 1. Admin Dashboard KPI Statistics
        /// </summary>
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var verifiedUsers = await _context.Users.CountAsync(u => u.IsVerified);
            var maleUsers = await _context.Users.CountAsync(u => u.Gender == "Male");
            var femaleUsers = await _context.Users.CountAsync(u => u.Gender == "Female");
            var totalInterests = await _context.UserInterests.CountAsync();
            var acceptedInterests = await _context.UserInterests.CountAsync(i => i.Status == "Accepted");
            var totalMessages = await _context.ChatMessages.CountAsync();
            var totalStories = await _context.SuccessStories.CountAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Gender,
                    u.Religion,
                    u.Location,
                    u.IsVerified,
                    u.IsBlocked,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                totalUsers,
                verifiedUsers,
                pendingVerifications = totalUsers - verifiedUsers,
                maleUsers,
                femaleUsers,
                totalInterests,
                acceptedInterests,
                totalMessages,
                totalStories,
                recentUsers
            });
        }

        /// <summary>
        /// 2. User Management - Get All Users
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search = null,
            [FromQuery] string? religion = null,
            [FromQuery] string? gender = null,
            [FromQuery] bool? isVerified = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower().Trim();
                query = query.Where(u => u.Name.ToLower().Contains(s) || u.Email.ToLower().Contains(s) || u.Mobile.Contains(s) || u.Location.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(religion) && religion != "All")
            {
                query = query.Where(u => u.Religion == religion);
            }

            if (!string.IsNullOrEmpty(gender) && gender != "All")
            {
                query = query.Where(u => u.Gender == gender);
            }

            if (isVerified.HasValue)
            {
                query = query.Where(u => u.IsVerified == isVerified.Value);
            }

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Mobile,
                    u.Gender,
                    u.DateOfBirth,
                    Age = DateTime.UtcNow.Year - u.DateOfBirth.Year,
                    u.Religion,
                    u.Caste,
                    u.MotherTongue,
                    u.Location,
                    u.ProfilePhotoUrl,
                    u.IsVerified,
                    u.IsBlocked,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// 3. Toggle User Verification Status
        /// </summary>
        [HttpPost("users/{id}/toggle-verify")]
        public async Task<IActionResult> ToggleVerify(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.IsVerified = !user.IsVerified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User verification status updated to: {(user.IsVerified ? "Verified ✓" : "Unverified ✗")}",
                isVerified = user.IsVerified
            });
        }

        /// <summary>
        /// 4. Toggle User Ban/Block Status
        /// </summary>
        [HttpPost("users/{id}/toggle-block")]
        public async Task<IActionResult> ToggleBlock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User account status: {(user.IsBlocked ? "Blocked / Suspended ⛔" : "Active & Allowed ✓")}",
                isBlocked = user.IsBlocked
            });
        }

        /// <summary>
        /// 5. Delete User Account
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {user.Name} (ID: MS-{user.Id}) deleted successfully." });
        }

        /// <summary>
        /// 6. Manage Success Stories
        /// </summary>
        [HttpGet("stories")]
        public async Task<IActionResult> GetStories()
        {
            var stories = await _context.SuccessStories
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return Ok(stories);
        }

        [HttpPost("stories")]
        public async Task<IActionResult> CreateStory([FromBody] SuccessStory story)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            story.CreatedAt = DateTime.UtcNow;
            _context.SuccessStories.Add(story);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success story published successfully!", story });
        }

        [HttpDelete("stories/{id}")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var story = await _context.SuccessStories.FindAsync(id);
            if (story == null) return NotFound(new { message = "Story not found." });

            _context.SuccessStories.Remove(story);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success story deleted successfully." });
        }

        /// <summary>
        /// 7. Manage Master Data (Religions, Languages, Locations)
        /// </summary>
        [HttpPost("master-data/religion")]
        public async Task<IActionResult> AddReligion([FromBody] MasterReligion model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) return BadRequest(new { message = "Name is required." });
            _context.MasterReligions.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Religion added to master database.", model });
        }

        [HttpPost("master-data/mothertongue")]
        public async Task<IActionResult> AddMotherTongue([FromBody] MasterMotherTongue model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) return BadRequest(new { message = "Name is required." });
            _context.MasterMotherTongues.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Language added to master database.", model });
        }

        [HttpPost("master-data/location")]
        public async Task<IActionResult> AddLocation([FromBody] MasterLocation model)
        {
            if (string.IsNullOrWhiteSpace(model.CityName)) return BadRequest(new { message = "City name is required." });
            _context.MasterLocations.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { message = "City added to master database.", model });
        }
    }
}

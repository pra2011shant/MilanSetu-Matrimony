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
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("featured-profiles")]
        public async Task<IActionResult> GetFeaturedProfiles()
        {
            var profiles = await _context.Users
                .Include(u => u.UserProfiles)
                .Where(u => u.IsVerified)
                .OrderByDescending(u => u.CreatedAt)
                .Take(8)
                .Select(u => new
                {
                    id = "MS-" + u.Id.ToString("D3"),
                    numericId = u.Id,
                    name = u.Name,
                    gender = u.Gender,
                    age = DateTime.UtcNow.Year - u.DateOfBirth.Year,
                    height = u.UserProfiles.Select(p => p.Height).FirstOrDefault() ?? "5'6\"",
                    religion = u.Religion,
                    caste = u.Caste ?? "General",
                    motherTongue = u.MotherTongue,
                    education = u.UserProfiles.Select(p => p.HighestEducation).FirstOrDefault() ?? "Graduate",
                    profession = u.UserProfiles.Select(p => p.Occupation).FirstOrDefault() ?? "Working Professional",
                    company = u.UserProfiles.Select(p => p.CompanyName).FirstOrDefault() ?? "Reputed Org",
                    location = u.Location,
                    imageUrl = !string.IsNullOrEmpty(u.ProfilePhotoUrl) ? u.ProfilePhotoUrl : (u.Gender == "Female" ? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80" : "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80"),
                    verified = u.IsVerified,
                    premium = true,
                    about = u.UserProfiles.Select(p => p.AboutMe).FirstOrDefault() ?? "Looking for a caring, understanding life partner.",
                    interestSent = false
                })
                .ToListAsync();

            return Ok(profiles);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPlatformStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalStories = await _context.SuccessStories.CountAsync();
            var totalInterests = await _context.UserInterests.CountAsync();

            return Ok(new
            {
                verifiedProfiles = Math.Max(totalUsers, 10000) + "+",
                happyMarriages = Math.Max(totalStories * 500, 4500) + "+",
                matchAccuracy = "98%",
                communities = "100+"
            });
        }
    }
}

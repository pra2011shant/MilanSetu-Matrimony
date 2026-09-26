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
    public class SuccessStoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuccessStoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuccessStories()
        {
            var stories = await _context.SuccessStories
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();

            if (!stories.Any())
            {
                // Seed initial DB records if empty
                var defaultStories = new List<SuccessStory>
                {
                    new SuccessStory
                    {
                        CoupleName = "Vikram & Radhika",
                        WeddingDate = "December 2025",
                        Location = "Jaipur Palace, Rajasthan",
                        ImageUrl = "https://images.unsplash.com/photo-1583939003579-730e3918a45a?auto=format&fit=crop&w=600&q=80",
                        Quote = "\"We connected on MilanSetu with just one click, and found a lifetime of unconditional love and laughter!\"",
                        StorySnippet = "Vikram from Pune and Radhika from Jaipur matched through verified filters. Their shared love for travel and family values led to a beautiful destination wedding.",
                        IsActive = true,
                        SortOrder = 1
                    },
                    new SuccessStory
                    {
                        CoupleName = "Aman & Harpreet",
                        WeddingDate = "November 2025",
                        Location = "Amritsar, Punjab",
                        ImageUrl = "https://images.unsplash.com/photo-1609357605129-26f69add5d6e?auto=format&fit=crop&w=600&q=80",
                        Quote = "\"MilanSetu’s verified profiles gave our families 100% peace of mind and the perfect life companion.\"",
                        StorySnippet = "Both working in healthcare, they found true alignment in aspirations and core Punjabi values within 3 weeks of connecting on the portal.",
                        IsActive = true,
                        SortOrder = 2
                    },
                    new SuccessStory
                    {
                        CoupleName = "Arjun & Sneha",
                        WeddingDate = "January 2026",
                        Location = "Udaipur, Rajasthan",
                        ImageUrl = "https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=600&q=80",
                        Quote = "\"Found my soulmate who understands my career goals and cherishes cultural traditions equally.\"",
                        StorySnippet = "From our first chat on MilanSetu to meeting each other’s families, everything felt naturally right. Forever grateful!",
                        IsActive = true,
                        SortOrder = 3
                    }
                };

                _context.SuccessStories.AddRange(defaultStories);
                await _context.SaveChangesAsync();
                return Ok(defaultStories);
            }

            return Ok(stories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory([FromBody] SuccessStory story)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.SuccessStories.Add(story);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSuccessStories), new { id = story.Id }, story);
        }
    }
}

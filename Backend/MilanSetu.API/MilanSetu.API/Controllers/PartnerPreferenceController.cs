using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnerPreferenceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartnerPreferenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerPreference()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null) userId = firstUser.Id;
            }

            var pref = await _context.PartnerPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pref == null)
            {
                var user = await _context.Users.FindAsync(userId);
                var defaultGender = user?.Gender == "Female" ? "Groom" : "Bride";

                pref = new PartnerPreference
                {
                    UserId = userId,
                    MinAge = user?.Gender == "Female" ? 24 : 21,
                    MaxAge = user?.Gender == "Female" ? 32 : 28,
                    MinHeight = "5'2\"",
                    MaxHeight = "6'0\"",
                    Religion = user?.Religion ?? "Any Religion",
                    Community = user?.Caste ?? "Open to All / Caste No Bar",
                    MotherTongue = user?.MotherTongue ?? "Any Language",
                    Education = "Graduate / Post Graduate / Professional Degree",
                    Profession = "Private / Govt / Business Professional",
                    MinAnnualIncome = "₹7 Lakhs & Above",
                    PreferredLocation = "Any Metro City / Open to Relocate",
                    MaritalStatus = "Never Married",
                    Diet = "Vegetarian / Eggetarian",
                    Drink = "Non-Drinker / Social Drinker",
                    Smoke = "Non-Smoker",
                    UpdatedAt = DateTime.UtcNow
                };

                if (userId > 0)
                {
                    _context.PartnerPreferences.Add(pref);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(MapToDto(pref));
        }

        [HttpPut]
        public async Task<IActionResult> SavePartnerPreference([FromBody] PartnerPreferenceDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0 && dto.UserId > 0) userId = dto.UserId;
            if (userId == 0)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null) userId = firstUser.Id;
            }

            var pref = await _context.PartnerPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pref == null)
            {
                pref = new PartnerPreference { UserId = userId };
                _context.PartnerPreferences.Add(pref);
            }

            pref.MinAge = dto.MinAge;
            pref.MaxAge = dto.MaxAge;
            pref.MinHeight = dto.MinHeight;
            pref.MaxHeight = dto.MaxHeight;
            pref.Religion = dto.Religion;
            pref.Community = dto.Community;
            pref.MotherTongue = dto.MotherTongue;
            pref.Education = dto.Education;
            pref.Profession = dto.Profession;
            pref.MinAnnualIncome = dto.MinAnnualIncome;
            pref.PreferredLocation = dto.PreferredLocation;
            pref.MaritalStatus = dto.MaritalStatus;
            pref.Diet = dto.Diet;
            pref.Drink = dto.Drink;
            pref.Smoke = dto.Smoke;
            pref.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Partner preferences successfully saved to your profile!",
                preferences = MapToDto(pref)
            });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private static PartnerPreferenceDto MapToDto(PartnerPreference p)
        {
            return new PartnerPreferenceDto
            {
                UserId = p.UserId,
                MinAge = p.MinAge,
                MaxAge = p.MaxAge,
                MinHeight = p.MinHeight,
                MaxHeight = p.MaxHeight,
                Religion = p.Religion,
                Community = p.Community,
                MotherTongue = p.MotherTongue,
                Education = p.Education,
                Profession = p.Profession,
                MinAnnualIncome = p.MinAnnualIncome,
                PreferredLocation = p.PreferredLocation,
                MaritalStatus = p.MaritalStatus,
                Diet = p.Diet,
                Drink = p.Drink,
                Smoke = p.Smoke,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}

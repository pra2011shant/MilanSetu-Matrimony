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
    public class MasterDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MasterDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMasterData()
        {
            var religions = await _context.MasterReligions
                .Where(r => r.IsActive)
                .OrderBy(r => r.SortOrder)
                .Select(r => r.Name)
                .ToListAsync();

            var motherTongues = await _context.MasterMotherTongues
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m => m.Name)
                .ToListAsync();

            var educations = await _context.MasterEducations
                .Where(e => e.IsActive)
                .OrderBy(e => e.SortOrder)
                .Select(e => e.DegreeName)
                .ToListAsync();

            var occupations = await _context.MasterOccupations
                .Where(o => o.IsActive)
                .OrderBy(o => o.SortOrder)
                .Select(o => o.Title)
                .ToListAsync();

            var incomeRanges = await _context.MasterIncomeRanges
                .Where(i => i.IsActive)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.RangeText)
                .ToListAsync();

            var locations = await _context.MasterLocations
                .Where(l => l.IsPopular)
                .OrderBy(l => l.SortOrder)
                .Select(l => $"{l.CityName}, {l.StateName}")
                .ToListAsync();

            // If empty, return standard master dictionary and auto-seed database
            if (!religions.Any())
            {
                religions = new List<string> { "Hindu", "Muslim", "Sikh", "Christian", "Jain", "Buddhist", "Parsi", "Jewish", "Other" };
                _context.MasterReligions.AddRange(religions.Select((r, i) => new MasterReligion { Name = r, SortOrder = i + 1, IsActive = true }));
            }

            if (!motherTongues.Any())
            {
                motherTongues = new List<string> { "Hindi", "Punjabi", "Bengali", "Marathi", "Gujarati", "Tamil", "Telugu", "Kannada", "Malayalam", "Odia", "Marwari", "Assamese", "Urdu", "English" };
                _context.MasterMotherTongues.AddRange(motherTongues.Select((m, i) => new MasterMotherTongue { Name = m, SortOrder = i + 1, IsActive = true }));
            }

            if (!educations.Any())
            {
                educations = new List<string> { "Graduate / Bachelor’s", "Post Graduate / Master’s", "Engineering / B.Tech / M.Tech", "Management / MBA / PGDM", "Medical / Doctor / MBBS", "Finance / CA / CS / CFA", "Doctorate / Ph.D." };
                _context.MasterEducations.AddRange(educations.Select((e, i) => new MasterEducation { DegreeName = e, SortOrder = i + 1, IsActive = true }));
            }

            if (!occupations.Any())
            {
                occupations = new List<string> { "Software / IT / Tech", "Govt / Civil Services / PSU", "Banking / Finance / Investment", "Healthcare / Doctor / Medical", "Business / Entrepreneur", "Architecture / Design", "Corporate Executive / Management" };
                _context.MasterOccupations.AddRange(occupations.Select((o, i) => new MasterOccupation { Title = o, SortOrder = i + 1, IsActive = true }));
            }

            if (!incomeRanges.Any())
            {
                incomeRanges = new List<string> { "Under ₹5 Lakhs", "₹5 - ₹10 Lakhs", "₹10 - ₹15 Lakhs", "₹15 - ₹25 Lakhs", "₹25 - ₹50 Lakhs", "₹50 Lakhs - ₹1 Crore", "₹1 Crore & Above" };
                _context.MasterIncomeRanges.AddRange(incomeRanges.Select((inc, i) => new MasterIncomeRange { RangeText = inc, SortOrder = i + 1, IsActive = true }));
            }

            if (!locations.Any())
            {
                var locData = new List<(string City, string State)>
                {
                    ("Mumbai", "Maharashtra"),
                    ("Delhi NCR", "Delhi"),
                    ("Bengaluru", "Karnataka"),
                    ("Pune", "Maharashtra"),
                    ("Hyderabad", "Telangana"),
                    ("Chennai", "Tamil Nadu"),
                    ("Kolkata", "West Bengal"),
                    ("Ahmedabad", "Gujarat"),
                    ("Jaipur", "Rajasthan"),
                    ("Lucknow", "Uttar Pradesh"),
                    ("Chandigarh", "Punjab / Haryana")
                };
                locations = locData.Select(l => $"{l.City}, {l.State}").ToList();
                _context.MasterLocations.AddRange(locData.Select((l, i) => new MasterLocation { CityName = l.City, StateName = l.State, SortOrder = i + 1, IsPopular = true }));
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                religions,
                motherTongues,
                educations,
                occupations,
                incomeRanges,
                locations
            });
        }
    }
}

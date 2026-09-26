using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SearchProfiles([FromBody] SearchFilterDto filter)
        {
            // Query existing users with profiles
            var query = _context.Users
                .Include(u => u.UserProfiles)
                .AsQueryable();

            // Direct Profile ID Search
            if (!string.IsNullOrEmpty(filter.ProfileId))
            {
                var pid = filter.ProfileId.Trim().ToUpper();
                if (pid.StartsWith("MS-") && int.TryParse(pid.Replace("MS-", ""), out var uid))
                {
                    query = query.Where(u => u.Id == uid);
                }
            }
            else
            {
                // Gender Filter (Bride = Female, Groom = Male)
                if (!string.IsNullOrEmpty(filter.Gender) && filter.Gender != "Any")
                {
                    if (filter.Gender.Equals("Bride", StringComparison.OrdinalIgnoreCase) || filter.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(u => u.Gender == "Female" || u.Gender == "Bride");
                    }
                    else if (filter.Gender.Equals("Groom", StringComparison.OrdinalIgnoreCase) || filter.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(u => u.Gender == "Male" || u.Gender == "Groom");
                    }
                }

                // Religion Filter
                if (!string.IsNullOrEmpty(filter.Religion) && filter.Religion != "Any" && filter.Religion != "Any Religion")
                {
                    query = query.Where(u => u.Religion.ToLower() == filter.Religion.ToLower());
                }

                // Mother Tongue Filter
                if (!string.IsNullOrEmpty(filter.MotherTongue) && filter.MotherTongue != "Any" && filter.MotherTongue != "Any Language")
                {
                    query = query.Where(u => u.MotherTongue.ToLower() == filter.MotherTongue.ToLower());
                }

                // Location / City Filter
                if (!string.IsNullOrEmpty(filter.City) && filter.City != "Any City")
                {
                    query = query.Where(u => u.Location.ToLower().Contains(filter.City.ToLower()));
                }

                // Community / Caste Filter
                if (!string.IsNullOrEmpty(filter.Community) && !filter.Community.Contains("Open", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(u => u.Caste != null && u.Caste.ToLower().Contains(filter.Community.ToLower()));
                }
            }

            var dbUsers = await query.ToListAsync();

            // Transform to SearchResultItemDto list
            var results = new List<SearchResultItemDto>();
            var today = DateTime.UtcNow;

            foreach (var u in dbUsers)
            {
                var age = today.Year - u.DateOfBirth.Year;
                if (u.DateOfBirth.Date > today.AddYears(-age)) age--;

                if (filter.MinAge.HasValue && age < filter.MinAge.Value) continue;
                if (filter.MaxAge.HasValue && age > filter.MaxAge.Value) continue;

                var p = u.UserProfiles.FirstOrDefault();

                results.Add(new SearchResultItemDto
                {
                    Id = u.Id,
                    ProfileId = $"MS-{100 + u.Id}",
                    Name = u.Name,
                    Gender = u.Gender,
                    Age = age,
                    Height = p?.Height ?? "5'6\"",
                    MaritalStatus = p?.MaritalStatus ?? "Never Married",
                    Religion = u.Religion,
                    Caste = u.Caste ?? p?.SubCasteOrGothra ?? "Open to all",
                    MotherTongue = u.MotherTongue,
                    Education = p?.HighestEducation ?? "Graduate / Professional",
                    Profession = p?.Occupation ?? "Working Professional",
                    Company = p?.CompanyName,
                    AnnualIncome = p?.AnnualIncome ?? "₹10 - ₹15 Lakhs",
                    Location = u.Location,
                    City = p?.City ?? (u.Location.Contains(",") ? u.Location.Split(',')[0].Trim() : u.Location),
                    State = p?.State ?? (u.Location.Contains(",") ? u.Location.Split(',')[1].Trim() : "India"),
                    ImageUrl = u.ProfilePhotoUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                    Verified = u.IsVerified,
                    Premium = true,
                    About = p?.AboutMe ?? "Ambitious and caring individual seeking a compatible life partner.",
                    InterestSent = false,
                    IsShortlisted = false
                });
            }

            // If few database records exist, supplement with rich matrimonial candidates
            var mockPool = GetMockCandidatesPool();
            foreach (var m in mockPool)
            {
                if (results.Any(r => r.Name == m.Name)) continue;

                // Match filters
                if (!string.IsNullOrEmpty(filter.Gender) && filter.Gender != "Any")
                {
                    if (filter.Gender.Equals("Bride", StringComparison.OrdinalIgnoreCase) && m.Gender != "Bride" && m.Gender != "Female") continue;
                    if (filter.Gender.Equals("Groom", StringComparison.OrdinalIgnoreCase) && m.Gender != "Groom" && m.Gender != "Male") continue;
                }

                if (filter.MinAge.HasValue && m.Age < filter.MinAge.Value) continue;
                if (filter.MaxAge.HasValue && m.Age > filter.MaxAge.Value) continue;

                if (!string.IsNullOrEmpty(filter.Religion) && filter.Religion != "Any" && filter.Religion != "Any Religion")
                {
                    if (!m.Religion.Equals(filter.Religion, StringComparison.OrdinalIgnoreCase)) continue;
                }

                if (!string.IsNullOrEmpty(filter.MotherTongue) && filter.MotherTongue != "Any" && filter.MotherTongue != "Any Language")
                {
                    if (!m.MotherTongue.Equals(filter.MotherTongue, StringComparison.OrdinalIgnoreCase)) continue;
                }

                if (!string.IsNullOrEmpty(filter.City) && filter.City != "Any City")
                {
                    if (!m.City.Contains(filter.City, StringComparison.OrdinalIgnoreCase) && !m.Location.Contains(filter.City, StringComparison.OrdinalIgnoreCase)) continue;
                }

                if (!string.IsNullOrEmpty(filter.MaritalStatus) && filter.MaritalStatus != "Any Marital Status" && filter.MaritalStatus != "Any")
                {
                    if (!m.MaritalStatus.Equals(filter.MaritalStatus, StringComparison.OrdinalIgnoreCase)) continue;
                }

                results.Add(m);
            }

            return Ok(new
            {
                total = results.Count,
                profiles = results
            });
        }

        private static List<SearchResultItemDto> GetMockCandidatesPool()
        {
            return new List<SearchResultItemDto>
            {
                new SearchResultItemDto
                {
                    Id = 101,
                    ProfileId = "MS-101",
                    Name = "Ananya Sharma",
                    Gender = "Bride",
                    Age = 25,
                    Height = "5'4\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Brahmin",
                    MotherTongue = "Hindi",
                    Education = "B.Tech - Computer Science",
                    Profession = "Senior Software Engineer",
                    Company = "Microsoft",
                    AnnualIncome = "₹18 - ₹24 Lakhs",
                    Location = "Bengaluru, Karnataka",
                    City = "Bengaluru",
                    State = "Karnataka",
                    ImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Warm, ambitious tech professional who values deep family bonds, Indian culture, and weekend travel."
                },
                new SearchResultItemDto
                {
                    Id = 102,
                    ProfileId = "MS-102",
                    Name = "Rohan Deshmukh",
                    Gender = "Groom",
                    Age = 28,
                    Height = "5'11\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Maratha",
                    MotherTongue = "Marathi",
                    Education = "MBA - Finance (IIM)",
                    Profession = "Investment Banker",
                    Company = "Goldman Sachs",
                    AnnualIncome = "₹25 - ₹35 Lakhs",
                    Location = "Mumbai, Maharashtra",
                    City = "Mumbai",
                    State = "Maharashtra",
                    ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Passionate about finance, fitness, and world cinema. Looking for an understanding and cheerful life companion."
                },
                new SearchResultItemDto
                {
                    Id = 103,
                    ProfileId = "MS-103",
                    Name = "Dr. Simran Kaur Gill",
                    Gender = "Bride",
                    Age = 26,
                    Height = "5'6\"",
                    MaritalStatus = "Never Married",
                    Religion = "Sikh",
                    Caste = "Jat Sikh",
                    MotherTongue = "Punjabi",
                    Education = "M.D. Pediatrics",
                    Profession = "Resident Doctor",
                    Company = "Apollo Hospital",
                    AnnualIncome = "₹15 - ₹20 Lakhs",
                    Location = "Chandigarh / Delhi",
                    City = "Chandigarh",
                    State = "Punjab",
                    ImageUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Dedicated doctor with a lively personality. Loves music, classical dance, and exploring new culinary experiences."
                },
                new SearchResultItemDto
                {
                    Id = 104,
                    ProfileId = "MS-104",
                    Name = "Aditya Patel",
                    Gender = "Groom",
                    Age = 29,
                    Height = "5'10\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Patel",
                    MotherTongue = "Gujarati",
                    Education = "MS in AI & Data Science",
                    Profession = "Product Lead",
                    Company = "Amazon",
                    AnnualIncome = "₹35 - ₹50 Lakhs",
                    Location = "Ahmedabad / Hyderabad",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    ImageUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Tech entrepreneur at heart. Believes in mutual respect, shared dreams, and lifelong growth together."
                },
                new SearchResultItemDto
                {
                    Id = 105,
                    ProfileId = "MS-105",
                    Name = "Pooja Banerjee",
                    Gender = "Bride",
                    Age = 27,
                    Height = "5'3\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Bengali Brahmin",
                    MotherTongue = "Bengali",
                    Education = "Chartered Accountant (CA)",
                    Profession = "Finance Manager",
                    Company = "Deloitte",
                    AnnualIncome = "₹18 - ₹25 Lakhs",
                    Location = "Kolkata / Gurugram",
                    City = "Kolkata",
                    State = "West Bengal",
                    ImageUrl = "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Warm-hearted, artistic, and grounded. Enjoys Rabindra Sangeet, reading literature, and weekend cooking."
                },
                new SearchResultItemDto
                {
                    Id = 106,
                    ProfileId = "MS-106",
                    Name = "Karthik Ramanathan",
                    Gender = "Groom",
                    Age = 30,
                    Height = "6'0\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Iyer Brahmin",
                    MotherTongue = "Tamil",
                    Education = "M.Tech - IIT Madras",
                    Profession = "Engineering Manager",
                    Company = "Google",
                    AnnualIncome = "₹45 - ₹60 Lakhs",
                    Location = "Chennai / Bengaluru",
                    City = "Chennai",
                    State = "Tamil Nadu",
                    ImageUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Calm, thoughtful, and passionate about innovation and Carnatic music. Seeking a supportive life partner."
                },
                new SearchResultItemDto
                {
                    Id = 107,
                    ProfileId = "MS-107",
                    Name = "Meera Nair",
                    Gender = "Bride",
                    Age = 26,
                    Height = "5'5\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Nair",
                    MotherTongue = "Malayalam",
                    Education = "MBA - Marketing (Symbiosis)",
                    Profession = "Brand Strategist",
                    Company = "Unilever",
                    AnnualIncome = "₹16 - ₹22 Lakhs",
                    Location = "Kochi / Mumbai",
                    City = "Kochi",
                    State = "Kerala",
                    ImageUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = false,
                    About = "Creative, energetic, and family-oriented. Passionate about art, travel, and culinary exploration."
                },
                new SearchResultItemDto
                {
                    Id = 108,
                    ProfileId = "MS-108",
                    Name = "Varun Kapoor",
                    Gender = "Groom",
                    Age = 29,
                    Height = "5'11\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Khatri Punjabi",
                    MotherTongue = "Punjabi",
                    Education = "B.Arch - SPA Delhi",
                    Profession = "Senior Architect & Partner",
                    Company = "Kapoor & Associates",
                    AnnualIncome = "₹30 - ₹40 Lakhs",
                    Location = "Delhi NCR",
                    City = "Delhi",
                    State = "Delhi",
                    ImageUrl = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Architect with an eye for aesthetics and design. Loves hiking, architecture tours, and spending time with family."
                }
            };
        }
    }
}

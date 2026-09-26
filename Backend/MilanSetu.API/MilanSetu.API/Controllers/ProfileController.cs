using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                // Fallback for non-authenticated testing: get the first user or create dummy
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null) userId = firstUser.Id;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                // Seed default profile for user
                profile = new UserProfile
                {
                    UserId = user.Id,
                    Height = "5'7\"",
                    Weight = "65 kg",
                    MaritalStatus = "Never Married",
                    PhysicalStatus = "Normal",
                    ProfileManagedBy = "Self",
                    AboutMe = "I am a warm, values-driven individual with modern outlook and deep cultural roots.",
                    PartnerExpectations = "Looking for an understanding, educated, and caring life companion.",
                    HighestEducation = "B.Tech / Master's Degree",
                    CollegeOrUniversity = "Top University",
                    FieldOfStudy = "Computer Science / Engineering",
                    EmployedIn = "Private Sector",
                    Occupation = "Software Professional",
                    CompanyName = "MNC / Tech Firm",
                    WorkLocation = user.Location,
                    AnnualIncome = "₹12 - ₹18 Lakhs",
                    FamilyType = "Nuclear",
                    FamilyValues = "Moderate",
                    FatherOccupation = "Employed / Retired",
                    MotherOccupation = "Homemaker",
                    NumberOfBrothers = 1,
                    NumberOfSisters = 0,
                    FamilyCity = user.Location,
                    Diet = "Vegetarian",
                    Drink = "No",
                    Smoke = "No",
                    Hobbies = "Travelling, Photography, Music, Reading, Fitness",
                    SubCasteOrGothra = user.Caste,
                    ManglikStatus = "No",
                    Rashi = "Tula (Libra)",
                    Nakshatra = "Swati",
                    City = user.Location.Contains(",") ? user.Location.Split(',')[0].Trim() : user.Location,
                    State = user.Location.Contains(",") ? user.Location.Split(',')[1].Trim() : "India",
                    Country = "India",
                    NativePlace = user.Location,
                    WillingToRelocate = true,
                    PhotoGalleryJson = JsonSerializer.Serialize(new List<string>
                    {
                        user.ProfilePhotoUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                        "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
                        "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80"
                    }),
                    ProfileCompletionPercentage = 90,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            var photos = !string.IsNullOrEmpty(profile.PhotoGalleryJson)
                ? JsonSerializer.Deserialize<List<string>>(profile.PhotoGalleryJson) ?? new List<string>()
                : new List<string>();

            var dto = MapToDto(user, profile, photos);
            return Ok(dto);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0 && dto.UserId > 0) userId = dto.UserId;
            if (userId == 0)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null) userId = firstUser.Id;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                profile = new UserProfile { UserId = userId };
                _context.UserProfiles.Add(profile);
            }

            // Update User fields if provided
            if (!string.IsNullOrEmpty(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Religion)) user.Religion = dto.Religion;
            if (!string.IsNullOrEmpty(dto.Caste)) user.Caste = dto.Caste;
            if (!string.IsNullOrEmpty(dto.MotherTongue)) user.MotherTongue = dto.MotherTongue;
            if (!string.IsNullOrEmpty(dto.ProfilePhotoUrl)) user.ProfilePhotoUrl = dto.ProfilePhotoUrl;

            // 1. Basic Information
            profile.Height = dto.Height;
            profile.Weight = dto.Weight;
            profile.MaritalStatus = dto.MaritalStatus;
            profile.PhysicalStatus = dto.PhysicalStatus;
            profile.ProfileManagedBy = dto.ProfileManagedBy;

            // 2. About Me
            profile.AboutMe = dto.AboutMe;
            profile.PartnerExpectations = dto.PartnerExpectations;

            // 3. Education
            profile.HighestEducation = dto.HighestEducation;
            profile.CollegeOrUniversity = dto.CollegeOrUniversity;
            profile.FieldOfStudy = dto.FieldOfStudy;

            // 4. Profession
            profile.EmployedIn = dto.EmployedIn;
            profile.Occupation = dto.Occupation;
            profile.CompanyName = dto.CompanyName;
            profile.WorkLocation = dto.WorkLocation;

            // 5. Income
            profile.AnnualIncome = dto.AnnualIncome;

            // 6. Family Details
            profile.FamilyType = dto.FamilyType;
            profile.FamilyValues = dto.FamilyValues;
            profile.FatherOccupation = dto.FatherOccupation;
            profile.MotherOccupation = dto.MotherOccupation;
            profile.NumberOfBrothers = dto.NumberOfBrothers;
            profile.NumberOfSisters = dto.NumberOfSisters;
            profile.FamilyCity = dto.FamilyCity;

            // 7. Lifestyle
            profile.Diet = dto.Diet;
            profile.Drink = dto.Drink;
            profile.Smoke = dto.Smoke;

            // 8. Hobbies
            profile.Hobbies = dto.Hobbies;

            // 9. Religion & Astrology
            profile.SubCasteOrGothra = dto.SubCasteOrGothra;
            profile.ManglikStatus = dto.ManglikStatus;
            profile.Rashi = dto.Rashi;
            profile.Nakshatra = dto.Nakshatra;

            // 10. Location
            profile.City = dto.City;
            profile.State = dto.State;
            profile.Country = dto.Country ?? "India";
            profile.NativePlace = dto.NativePlace;
            profile.WillingToRelocate = dto.WillingToRelocate;

            // 11. Photos
            if (dto.PhotoGallery != null && dto.PhotoGallery.Any())
            {
                profile.PhotoGalleryJson = JsonSerializer.Serialize(dto.PhotoGallery);
                if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
                {
                    user.ProfilePhotoUrl = dto.PhotoGallery.First();
                }
            }

            profile.ProfileCompletionPercentage = CalculateCompletion(profile, user);
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var photos = !string.IsNullOrEmpty(profile.PhotoGalleryJson)
                ? JsonSerializer.Deserialize<List<string>>(profile.PhotoGalleryJson) ?? new List<string>()
                : new List<string>();

            return Ok(new
            {
                message = "Profile successfully updated!",
                profile = MapToDto(user, profile, photos)
            });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private static int CalculateCompletion(UserProfile p, User u)
        {
            int score = 30; // base score for registration
            if (!string.IsNullOrEmpty(p.AboutMe)) score += 10;
            if (!string.IsNullOrEmpty(p.HighestEducation)) score += 10;
            if (!string.IsNullOrEmpty(p.Occupation)) score += 10;
            if (!string.IsNullOrEmpty(p.AnnualIncome)) score += 5;
            if (!string.IsNullOrEmpty(p.FatherOccupation)) score += 10;
            if (!string.IsNullOrEmpty(p.Diet)) score += 5;
            if (!string.IsNullOrEmpty(p.Hobbies)) score += 5;
            if (!string.IsNullOrEmpty(p.SubCasteOrGothra)) score += 5;
            if (!string.IsNullOrEmpty(p.PhotoGalleryJson) && p.PhotoGalleryJson.Length > 20) score += 10;
            return Math.Min(score, 100);
        }

        private static UserProfileDto MapToDto(User user, UserProfile p, List<string> photos)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth.Date > today.AddYears(-age)) age--;

            return new UserProfileDto
            {
                UserId = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                Email = user.Email,
                Mobile = user.Mobile,
                Religion = user.Religion,
                Caste = user.Caste,
                MotherTongue = user.MotherTongue,
                ProfilePhotoUrl = user.ProfilePhotoUrl ?? photos.FirstOrDefault(),
                IsVerified = user.IsVerified,
                Height = p.Height,
                Weight = p.Weight,
                MaritalStatus = p.MaritalStatus,
                PhysicalStatus = p.PhysicalStatus,
                ProfileManagedBy = p.ProfileManagedBy,
                AboutMe = p.AboutMe,
                PartnerExpectations = p.PartnerExpectations,
                HighestEducation = p.HighestEducation,
                CollegeOrUniversity = p.CollegeOrUniversity,
                FieldOfStudy = p.FieldOfStudy,
                EmployedIn = p.EmployedIn,
                Occupation = p.Occupation,
                CompanyName = p.CompanyName,
                WorkLocation = p.WorkLocation,
                AnnualIncome = p.AnnualIncome,
                FamilyType = p.FamilyType,
                FamilyValues = p.FamilyValues,
                FatherOccupation = p.FatherOccupation,
                MotherOccupation = p.MotherOccupation,
                NumberOfBrothers = p.NumberOfBrothers,
                NumberOfSisters = p.NumberOfSisters,
                FamilyCity = p.FamilyCity,
                Diet = p.Diet,
                Drink = p.Drink,
                Smoke = p.Smoke,
                Hobbies = p.Hobbies,
                SubCasteOrGothra = p.SubCasteOrGothra,
                ManglikStatus = p.ManglikStatus,
                Rashi = p.Rashi,
                Nakshatra = p.Nakshatra,
                City = p.City,
                State = p.State,
                Country = p.Country,
                NativePlace = p.NativePlace,
                WillingToRelocate = p.WillingToRelocate,
                PhotoGallery = photos,
                ProfileCompletionPercentage = p.ProfileCompletionPercentage,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}

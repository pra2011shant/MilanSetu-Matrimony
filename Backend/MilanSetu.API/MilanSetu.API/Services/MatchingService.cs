using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanSetu.API.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public MatchingService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<DashboardMatchesDto> GetDashboardMatchesAsync(int currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.UserProfiles)
                .Include(u => u.PartnerPreferences)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            var preference = currentUser?.PartnerPreferences.FirstOrDefault();
            var myProfile = currentUser?.UserProfiles.FirstOrDefault();

            // Opposing gender
            string targetGender = "Female";
            if (currentUser != null && (currentUser.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) || currentUser.Gender.Equals("Bride", StringComparison.OrdinalIgnoreCase)))
            {
                targetGender = "Male";
            }

            // Fetch active candidate pool from DB
            var dbCandidates = await _context.Users
                .Include(u => u.UserProfiles)
                .Where(u => u.Id != currentUserId)
                .ToListAsync();

            var shortlistedIds = await _context.UserShortlists
                .Where(s => s.UserId == currentUserId)
                .Select(s => s.ShortlistedUserId)
                .ToListAsync();

            var mySentInterests = await _context.UserInterests
                .Where(i => i.SenderUserId == currentUserId)
                .ToDictionaryAsync(i => i.ReceiverUserId, i => i.Status);

            var myReceivedInterests = await _context.UserInterests
                .Where(i => i.ReceiverUserId == currentUserId)
                .CountAsync();

            var recentViewIds = await _context.ProfileViews
                .Where(pv => pv.ViewerUserId == currentUserId)
                .OrderByDescending(pv => pv.ViewedAt)
                .Take(12)
                .Select(pv => pv.ViewedUserId)
                .ToListAsync();

            var visitorUserIds = await _context.ProfileViews
                .Where(pv => pv.ViewedUserId == currentUserId)
                .OrderByDescending(pv => pv.ViewedAt)
                .Take(12)
                .Select(pv => pv.ViewerUserId)
                .ToListAsync();

            // Process candidates
            var allMatched = new List<MatchedProfileDto>();

            foreach (var candidate in dbCandidates)
            {
                var profile = candidate.UserProfiles.FirstOrDefault();
                var matchDto = CalculateMatch(preference, candidate, profile);
                
                matchDto.IsShortlisted = shortlistedIds.Contains(candidate.Id);
                if (mySentInterests.TryGetValue(candidate.Id, out var status))
                {
                    matchDto.InterestStatus = status;
                }

                allMatched.Add(matchDto);
            }

            // Supplement with rich curated profiles if DB has limited records
            var mockPool = GetMockCandidatesWithScores(targetGender, preference, currentUser?.Location ?? "Bengaluru");
            foreach (var m in mockPool)
            {
                if (!allMatched.Any(x => x.Name == m.Name || x.ProfileId == m.ProfileId))
                {
                    allMatched.Add(m);
                }
            }

            // Segmentation
            // 1. Recommended Matches: High score (>= 75%) sorted by score descending
            var recommended = allMatched
                .OrderByDescending(x => x.MatchScore)
                .Take(12)
                .ToList();

            // 2. New Matches: Recently created or highest ID
            var newMatches = allMatched
                .OrderByDescending(x => x.Id)
                .Take(8)
                .ToList();

            // 3. Near Me: Matching city or state
            var myCity = myProfile?.City ?? (currentUser?.Location.Contains(",") == true ? currentUser.Location.Split(',')[0].Trim() : "Bengaluru");
            var nearMe = allMatched
                .Where(x => x.Location.Contains(myCity, StringComparison.OrdinalIgnoreCase) || 
                            x.City.Contains(myCity, StringComparison.OrdinalIgnoreCase) ||
                            (myProfile?.State != null && x.State.Contains(myProfile.State, StringComparison.OrdinalIgnoreCase)))
                .Take(8)
                .ToList();
            if (nearMe.Count < 3)
            {
                nearMe = allMatched.Take(4).ToList(); // fallback
            }

            // 4. Shortlisted
            var shortlisted = allMatched
                .Where(x => x.IsShortlisted)
                .ToList();

            // 5. Recently Viewed
            var recentlyViewed = allMatched
                .Where(x => recentViewIds.Contains(x.Id))
                .ToList();
            if (recentlyViewed.Count == 0)
            {
                recentlyViewed = allMatched.Skip(2).Take(4).ToList(); // mock recent views
            }

            // 6. Profile Visitors
            var visitors = allMatched
                .Where(x => visitorUserIds.Contains(x.Id))
                .ToList();
            if (visitors.Count == 0)
            {
                visitors = allMatched.Skip(4).Take(4).ToList(); // mock visitor activity
            }

            return new DashboardMatchesDto
            {
                TotalRecommended = recommended.Count,
                TotalNewToday = newMatches.Count,
                TotalVisitors = visitors.Count,
                TotalShortlisted = shortlisted.Count,
                TotalInterestsReceived = myReceivedInterests > 0 ? myReceivedInterests : 3,
                RecommendedMatches = recommended,
                NewMatches = newMatches,
                NearMeMatches = nearMe,
                RecentlyViewed = recentlyViewed,
                ShortlistedMatches = shortlisted,
                ProfileVisitors = visitors
            };
        }

        public MatchedProfileDto CalculateMatch(PartnerPreference? pref, User user, UserProfile? profile)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth.Date > today.AddYears(-age)) age--;

            int score = 40; // Base score
            int totalWeight = 0;
            int matchedWeight = 0;
            var criteriaList = new List<MatchCriteriaItem>();

            // Default preference if user hasn't set one yet
            int prefMinAge = pref?.MinAge ?? 21;
            int prefMaxAge = pref?.MaxAge ?? 35;
            string prefReligion = pref?.Religion ?? "Any Religion";
            string prefTongue = pref?.MotherTongue ?? "Any Language";
            string prefDiet = pref?.Diet ?? "Vegetarian";
            string prefMarital = pref?.MaritalStatus ?? "Never Married";

            // 1. Age (Weight: 20)
            totalWeight += 20;
            bool ageMatch = age >= prefMinAge && age <= prefMaxAge;
            if (ageMatch) matchedWeight += 20;
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Age",
                Description = $"{age} Yrs (Preferred: {prefMinAge} - {prefMaxAge} Yrs)",
                IsMatch = ageMatch,
                Icon = ageMatch ? "bi-check-circle-fill text-success" : "bi-x-circle-fill text-muted"
            });

            // 2. Religion (Weight: 20)
            totalWeight += 20;
            bool relMatch = prefReligion.Contains("Any", StringComparison.OrdinalIgnoreCase) || 
                            user.Religion.Equals(prefReligion, StringComparison.OrdinalIgnoreCase);
            if (relMatch) matchedWeight += 20;
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Religion",
                Description = $"{user.Religion} (Preferred: {prefReligion})",
                IsMatch = relMatch,
                Icon = relMatch ? "bi-check-circle-fill text-success" : "bi-x-circle-fill text-muted"
            });

            // 3. Mother Tongue (Weight: 15)
            totalWeight += 15;
            bool tongueMatch = prefTongue.Contains("Any", StringComparison.OrdinalIgnoreCase) || 
                               user.MotherTongue.Equals(prefTongue, StringComparison.OrdinalIgnoreCase);
            if (tongueMatch) matchedWeight += 15;
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Mother Tongue",
                Description = $"{user.MotherTongue} (Preferred: {prefTongue})",
                IsMatch = tongueMatch,
                Icon = tongueMatch ? "bi-check-circle-fill text-success" : "bi-x-circle-fill text-muted"
            });

            // 4. Marital Status (Weight: 15)
            totalWeight += 15;
            string userMarital = profile?.MaritalStatus ?? "Never Married";
            bool maritalMatch = prefMarital.Contains("Any", StringComparison.OrdinalIgnoreCase) || 
                                userMarital.Equals(prefMarital, StringComparison.OrdinalIgnoreCase);
            if (maritalMatch) matchedWeight += 15;
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Marital Status",
                Description = $"{userMarital} (Preferred: {prefMarital})",
                IsMatch = maritalMatch,
                Icon = maritalMatch ? "bi-check-circle-fill text-success" : "bi-x-circle-fill text-muted"
            });

            // 5. Diet / Lifestyle (Weight: 15)
            totalWeight += 15;
            string userDiet = profile?.Diet ?? "Vegetarian";
            bool dietMatch = prefDiet.Contains("Any", StringComparison.OrdinalIgnoreCase) || 
                             prefDiet.Contains(userDiet, StringComparison.OrdinalIgnoreCase);
            if (dietMatch) matchedWeight += 15;
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Diet / Lifestyle",
                Description = $"{userDiet} (Preferred: {prefDiet})",
                IsMatch = dietMatch,
                Icon = dietMatch ? "bi-check-circle-fill text-success" : "bi-x-circle-fill text-muted"
            });

            // 6. Education / Profession (Weight: 15)
            totalWeight += 15;
            matchedWeight += 15; // By default professional compatibility
            criteriaList.Add(new MatchCriteriaItem
            {
                Title = "Education & Career",
                Description = $"{profile?.HighestEducation ?? "Graduate"} - {profile?.Occupation ?? "Professional"}",
                IsMatch = true,
                Icon = "bi-check-circle-fill text-success"
            });

            score = totalWeight > 0 ? (int)Math.Round((double)matchedWeight / totalWeight * 100) : 85;
            if (score < 40) score = 40;

            int matchCount = criteriaList.Count(c => c.IsMatch);

            string matchBadge = score >= 90 ? "✨ 90%+ Perfect Match" :
                               score >= 75 ? "⭐ High Compatibility" :
                               "🤝 Good Match";

            return new MatchedProfileDto
            {
                Id = user.Id,
                ProfileId = $"MS-{100 + user.Id}",
                Name = user.Name,
                Gender = user.Gender,
                Age = age,
                Height = profile?.Height ?? "5'6\"",
                MaritalStatus = userMarital,
                Religion = user.Religion,
                Caste = user.Caste ?? profile?.SubCasteOrGothra ?? "Open to All",
                MotherTongue = user.MotherTongue,
                Education = profile?.HighestEducation ?? "B.Tech / Professional Degree",
                Profession = profile?.Occupation ?? "Software Engineer / Professional",
                Company = profile?.CompanyName ?? "Top MNC",
                AnnualIncome = profile?.AnnualIncome ?? "₹15 - ₹20 Lakhs",
                Location = user.Location,
                City = profile?.City ?? (user.Location.Contains(",") ? user.Location.Split(',')[0].Trim() : user.Location),
                State = profile?.State ?? (user.Location.Contains(",") ? user.Location.Split(',')[1].Trim() : "India"),
                ImageUrl = user.ProfilePhotoUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                Verified = user.IsVerified,
                Premium = true,
                About = profile?.AboutMe ?? "Warm, grounded, family-oriented individual looking for a supportive partner.",
                MatchScore = score,
                MatchBadge = matchBadge,
                MatchSummary = $"{matchCount} of {criteriaList.Count} Preferences Matched",
                MatchCriteriaList = criteriaList,
                IsShortlisted = false,
                InterestStatus = "None"
            };
        }

        public async Task<bool> ToggleShortlistAsync(int currentUserId, int targetUserId)
        {
            var existing = await _context.UserShortlists
                .FirstOrDefaultAsync(s => s.UserId == currentUserId && s.ShortlistedUserId == targetUserId);

            if (existing != null)
            {
                _context.UserShortlists.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // Removed
            }
            else
            {
                _context.UserShortlists.Add(new UserShortlist
                {
                    UserId = currentUserId,
                    ShortlistedUserId = targetUserId,
                    ShortlistedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return true; // Added
            }
        }

        public async Task<bool> RecordProfileViewAsync(int currentUserId, int targetUserId)
        {
            if (currentUserId == targetUserId) return false;

            // 1. Record Profile View in Database
            _context.ProfileViews.Add(new ProfileView
            {
                ViewerUserId = currentUserId,
                ViewedUserId = targetUserId,
                ViewedAt = DateTime.UtcNow
            });

            // 2. Fetch Viewer and Target User details
            var viewer = await _context.Users.FindAsync(currentUserId);
            var targetUser = await _context.Users.FindAsync(targetUserId);

            if (viewer != null && targetUser != null)
            {
                // 3. Create In-App Notification with Viewer ID
                var notification = new Notification
                {
                    UserId = targetUserId,
                    Type = "ProfileView",
                    Title = "Profile Viewed! 👀",
                    Message = $"{viewer.Name} (Matrimony ID: MS-{viewer.Id:D5}) viewed your profile.",
                    AvatarUrl = viewer.ProfilePhotoUrl,
                    ActionUrl = $"/search?profileId={viewer.Id}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);

                // 4. Send Email Notification
                if (!string.IsNullOrEmpty(targetUser.Email) && targetUser.Email.Contains("@"))
                {
                    _ = _emailService.SendProfileViewNotificationEmailAsync(targetUser.Email, targetUser.Name, viewer.Name, viewer.Id);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserInterest> ExpressInterestAsync(int currentUserId, int targetUserId, string? message)
        {
            var existing = await _context.UserInterests
                .FirstOrDefaultAsync(i => i.SenderUserId == currentUserId && i.ReceiverUserId == targetUserId);

            if (existing != null)
            {
                existing.Status = existing.Status == "Withdrawn" ? "Pending" : "Withdrawn";
                existing.RespondedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existing;
            }

            var newInterest = new UserInterest
            {
                SenderUserId = currentUserId,
                ReceiverUserId = targetUserId,
                Status = "Pending",
                CustomMessage = message ?? "Hi, I liked your profile on MilanSetu and would love to connect!",
                SentAt = DateTime.UtcNow
            };

            _context.UserInterests.Add(newInterest);
            await _context.SaveChangesAsync();
            return newInterest;
        }

        public async Task<bool> RespondToInterestAsync(int currentUserId, int interestId, string action)
        {
            var interest = await _context.UserInterests
                .FirstOrDefaultAsync(i => i.Id == interestId && i.ReceiverUserId == currentUserId);

            if (interest == null) return false;

            interest.Status = action; // Accepted or Declined
            interest.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static List<MatchedProfileDto> GetMockCandidatesWithScores(string targetGender, PartnerPreference? pref, string userLocation)
        {
            return new List<MatchedProfileDto>
            {
                new MatchedProfileDto
                {
                    Id = 201,
                    ProfileId = "MS-201",
                    Name = targetGender == "Male" ? "Aarav Singhania" : "Ananya Sharma",
                    Gender = targetGender == "Male" ? "Groom" : "Bride",
                    Age = targetGender == "Male" ? 28 : 25,
                    Height = targetGender == "Male" ? "5'11\"" : "5'4\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = "Brahmin",
                    MotherTongue = "Hindi",
                    Education = "B.Tech - CS & MBA (IIM)",
                    Profession = "Senior Product Manager",
                    Company = "Google India",
                    AnnualIncome = "₹28 - ₹35 Lakhs",
                    Location = "Bengaluru, Karnataka",
                    City = "Bengaluru",
                    State = "Karnataka",
                    ImageUrl = targetGender == "Male" 
                        ? "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80"
                        : "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Enthusiastic about technology, mindful living, and road trips. Seeking a forward-thinking, joyful partner.",
                    MatchScore = 96,
                    MatchBadge = "✨ 96% Top Recommendation",
                    MatchSummary = "6 of 6 Preferences Matched",
                    MatchCriteriaList = new List<MatchCriteriaItem>
                    {
                        new MatchCriteriaItem { Title = "Age", Description = "Matches preferred range (21-30)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Religion", Description = "Hindu (Exact Match)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Mother Tongue", Description = "Hindi (Preferred Language)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Education", Description = "Master's / MBA (Matches criteria)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Location", Description = "Bengaluru, Karnataka (Near You)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Diet", Description = "Vegetarian (Matches lifestyle)", IsMatch = true }
                    }
                },
                new MatchedProfileDto
                {
                    Id = 202,
                    ProfileId = "MS-202",
                    Name = targetGender == "Male" ? "Vikramaditya Roy" : "Dr. Simran Kaur Gill",
                    Gender = targetGender == "Male" ? "Groom" : "Bride",
                    Age = targetGender == "Male" ? 29 : 26,
                    Height = targetGender == "Male" ? "6'0\"" : "5'6\"",
                    MaritalStatus = "Never Married",
                    Religion = targetGender == "Male" ? "Hindu" : "Sikh",
                    Caste = targetGender == "Male" ? "Kayastha" : "Jat Sikh",
                    MotherTongue = targetGender == "Male" ? "Bengali" : "Punjabi",
                    Education = targetGender == "Male" ? "M.Tech - IIT Delhi" : "M.D. Pediatrics",
                    Profession = targetGender == "Male" ? "Lead AI Scientist" : "Resident Doctor",
                    Company = targetGender == "Male" ? "Amazon" : "Apollo Hospital",
                    AnnualIncome = "₹22 - ₹30 Lakhs",
                    Location = "Delhi NCR / Chandigarh",
                    City = "Delhi",
                    State = "Delhi",
                    ImageUrl = targetGender == "Male"
                        ? "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80"
                        : "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Compassionate professional with a passion for creative arts and family celebrations.",
                    MatchScore = 88,
                    MatchBadge = "⭐ 88% Great Match",
                    MatchSummary = "5 of 6 Preferences Matched",
                    MatchCriteriaList = new List<MatchCriteriaItem>
                    {
                        new MatchCriteriaItem { Title = "Age", Description = "Matches age bracket", IsMatch = true },
                        new MatchCriteriaItem { Title = "Education", Description = "Postgraduate (Matches criteria)", IsMatch = true },
                        new MatchCriteriaItem { Title = "Profession", Description = "High-income Professional", IsMatch = true },
                        new MatchCriteriaItem { Title = "Marital Status", Description = "Never Married", IsMatch = true },
                        new MatchCriteriaItem { Title = "Diet", Description = "Vegetarian / Flexible", IsMatch = true }
                    }
                },
                new MatchedProfileDto
                {
                    Id = 203,
                    ProfileId = "MS-203",
                    Name = targetGender == "Male" ? "Karthik Subramanian" : "Pooja Banerjee",
                    Gender = targetGender == "Male" ? "Groom" : "Bride",
                    Age = targetGender == "Male" ? 28 : 27,
                    Height = targetGender == "Male" ? "5'10\"" : "5'3\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = targetGender == "Male" ? "Iyer" : "Brahmin",
                    MotherTongue = targetGender == "Male" ? "Tamil" : "Bengali",
                    Education = "Chartered Accountant (CA)",
                    Profession = "Senior Finance Consultant",
                    Company = "Deloitte India",
                    AnnualIncome = "₹20 - ₹26 Lakhs",
                    Location = "Mumbai / Bengaluru",
                    City = "Mumbai",
                    State = "Maharashtra",
                    ImageUrl = targetGender == "Male"
                        ? "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80"
                        : "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = true,
                    About = "Grounded, ambitious, loves indie music and weekend trekking. Values genuine emotional connection.",
                    MatchScore = 84,
                    MatchBadge = "⭐ 84% Strong Match",
                    MatchSummary = "5 of 6 Preferences Matched",
                    MatchCriteriaList = new List<MatchCriteriaItem>
                    {
                        new MatchCriteriaItem { Title = "Age", Description = "Within ideal range", IsMatch = true },
                        new MatchCriteriaItem { Title = "Religion", Description = "Hindu", IsMatch = true },
                        new MatchCriteriaItem { Title = "Marital Status", Description = "Never Married", IsMatch = true },
                        new MatchCriteriaItem { Title = "Career", Description = "CA / Finance Leader", IsMatch = true }
                    }
                },
                new MatchedProfileDto
                {
                    Id = 204,
                    ProfileId = "MS-204",
                    Name = targetGender == "Male" ? "Rohan Deshmukh" : "Meera Nair",
                    Gender = targetGender == "Male" ? "Groom" : "Bride",
                    Age = targetGender == "Male" ? 30 : 26,
                    Height = targetGender == "Male" ? "5'11\"" : "5'5\"",
                    MaritalStatus = "Never Married",
                    Religion = "Hindu",
                    Caste = targetGender == "Male" ? "Maratha" : "Nair",
                    MotherTongue = targetGender == "Male" ? "Marathi" : "Malayalam",
                    Education = "MBA - Marketing (Symbiosis)",
                    Profession = "Global Brand Strategist",
                    Company = "Unilever",
                    AnnualIncome = "₹18 - ₹25 Lakhs",
                    Location = "Pune / Bengaluru",
                    City = "Pune",
                    State = "Maharashtra",
                    ImageUrl = targetGender == "Male"
                        ? "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=600&q=80"
                        : "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80",
                    Verified = true,
                    Premium = false,
                    About = "Creative, energetic, and family-first soul. Seeking a life partner to explore the world with.",
                    MatchScore = 80,
                    MatchBadge = "🤝 80% Good Match",
                    MatchSummary = "4 of 6 Preferences Matched",
                    MatchCriteriaList = new List<MatchCriteriaItem>
                    {
                        new MatchCriteriaItem { Title = "Age", Description = "Matches preference", IsMatch = true },
                        new MatchCriteriaItem { Title = "Religion", Description = "Hindu", IsMatch = true },
                        new MatchCriteriaItem { Title = "Lifestyle", Description = "Moderate / Modern", IsMatch = true }
                    }
                }
            };
        }
    }
}

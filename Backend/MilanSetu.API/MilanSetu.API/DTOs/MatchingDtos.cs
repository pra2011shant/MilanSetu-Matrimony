using System;
using System.Collections.Generic;

namespace MilanSetu.API.DTOs
{
    public class MatchCriteriaItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
        public string Icon { get; set; } = "bi-check-circle-fill";
    }

    public class MatchedProfileDto
    {
        public int Id { get; set; }
        public string ProfileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Height { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Caste { get; set; } = string.Empty;
        public string MotherTongue { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string AnnualIncome { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool Verified { get; set; } = true;
        public bool Premium { get; set; } = true;
        public string? About { get; set; }

        // Matching Engine Fields
        public int MatchScore { get; set; } = 85; // 0 to 100%
        public string MatchBadge { get; set; } = "Very High Compatibility"; // Perfect Match, Great Match, Good Match
        public string MatchSummary { get; set; } = "9 of 10 Preferences Matched";
        public List<MatchCriteriaItem> MatchCriteriaList { get; set; } = new List<MatchCriteriaItem>();

        // Engagement Status
        public bool IsShortlisted { get; set; }
        public string? InterestStatus { get; set; } // None, Pending, Accepted, Declined
        public DateTime? ActivityTimestamp { get; set; }
    }

    public class DashboardMatchesDto
    {
        public int TotalRecommended { get; set; }
        public int TotalNewToday { get; set; }
        public int TotalVisitors { get; set; }
        public int TotalShortlisted { get; set; }
        public int TotalInterestsReceived { get; set; }

        public List<MatchedProfileDto> RecommendedMatches { get; set; } = new List<MatchedProfileDto>();
        public List<MatchedProfileDto> NewMatches { get; set; } = new List<MatchedProfileDto>();
        public List<MatchedProfileDto> NearMeMatches { get; set; } = new List<MatchedProfileDto>();
        public List<MatchedProfileDto> RecentlyViewed { get; set; } = new List<MatchedProfileDto>();
        public List<MatchedProfileDto> ShortlistedMatches { get; set; } = new List<MatchedProfileDto>();
        public List<MatchedProfileDto> ProfileVisitors { get; set; } = new List<MatchedProfileDto>();
    }

    public class SendInterestRequestDto
    {
        public int TargetUserId { get; set; }
        public string? Message { get; set; }
    }

    public class RespondInterestDto
    {
        public string Action { get; set; } = "Accepted"; // Accepted, Declined
    }
}

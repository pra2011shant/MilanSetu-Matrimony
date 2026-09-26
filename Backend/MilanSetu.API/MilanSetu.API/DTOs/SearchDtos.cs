using System;
using System.Collections.Generic;

namespace MilanSetu.API.DTOs
{
    public class SearchFilterDto
    {
        public string? Gender { get; set; } = "Bride";
        public int? MinAge { get; set; } = 21;
        public int? MaxAge { get; set; } = 35;
        public string? MinHeight { get; set; }
        public string? MaxHeight { get; set; }
        public string? Religion { get; set; }
        public string? Community { get; set; }
        public string? MotherTongue { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Education { get; set; }
        public string? Profession { get; set; }
        public string? MaritalStatus { get; set; }
        public string? MinIncome { get; set; }
        public string? ProfileId { get; set; }
        public bool? PhotoOnly { get; set; }
        public bool? VerifiedOnly { get; set; }
        public string? SortBy { get; set; } = "relevance"; // relevance, age_asc, age_desc, newest
    }

    public class SearchResultItemDto
    {
        public int Id { get; set; }
        public string ProfileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Height { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string? Caste { get; set; }
        public string MotherTongue { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string AnnualIncome { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool Verified { get; set; }
        public bool Premium { get; set; }
        public string About { get; set; } = string.Empty;
        public bool InterestSent { get; set; }
        public bool IsShortlisted { get; set; }
    }
}

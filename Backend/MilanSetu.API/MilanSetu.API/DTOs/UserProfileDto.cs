using System;
using System.Collections.Generic;

namespace MilanSetu.API.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string? Caste { get; set; }
        public string MotherTongue { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }
        public bool IsVerified { get; set; }

        // 1. Basic Information
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? MaritalStatus { get; set; }
        public string? PhysicalStatus { get; set; }
        public string? ProfileManagedBy { get; set; }

        // 2. About Me
        public string? AboutMe { get; set; }
        public string? PartnerExpectations { get; set; }

        // 3. Education
        public string? HighestEducation { get; set; }
        public string? CollegeOrUniversity { get; set; }
        public string? FieldOfStudy { get; set; }

        // 4. Profession
        public string? EmployedIn { get; set; }
        public string? Occupation { get; set; }
        public string? CompanyName { get; set; }
        public string? WorkLocation { get; set; }

        // 5. Income
        public string? AnnualIncome { get; set; }

        // 6. Family Details
        public string? FamilyType { get; set; }
        public string? FamilyValues { get; set; }
        public string? FatherOccupation { get; set; }
        public string? MotherOccupation { get; set; }
        public int NumberOfBrothers { get; set; }
        public int NumberOfSisters { get; set; }
        public string? FamilyCity { get; set; }

        // 7. Lifestyle
        public string? Diet { get; set; }
        public string? Drink { get; set; }
        public string? Smoke { get; set; }

        // 8. Hobbies
        public string? Hobbies { get; set; }

        // 9. Religion & Astrology
        public string? SubCasteOrGothra { get; set; }
        public string? ManglikStatus { get; set; }
        public string? Rashi { get; set; }
        public string? Nakshatra { get; set; }

        // 10. Location
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? NativePlace { get; set; }
        public bool WillingToRelocate { get; set; }

        // 11. Photos
        public List<string> PhotoGallery { get; set; } = new List<string>();

        public int ProfileCompletionPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

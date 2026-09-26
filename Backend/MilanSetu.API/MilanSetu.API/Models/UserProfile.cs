using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[UserProfiles]
    /// 11-Section comprehensive matrimonial profile information including lifestyle, family, career, and astrology.
    /// </summary>
    [Table("UserProfiles")]
    public class UserProfile
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: UserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // -------------------------------------------------------------
        // SECTION 1: Basic Information
        // -------------------------------------------------------------
        [MaxLength(20)]
        public string? Height { get; set; } = "5'7\"";
        
        [MaxLength(20)]
        public string? Weight { get; set; } = "65 kg";
        
        [MaxLength(30)]
        public string? MaritalStatus { get; set; } = "Never Married";
        
        [MaxLength(30)]
        public string? PhysicalStatus { get; set; } = "Normal";
        
        [MaxLength(30)]
        public string? ProfileManagedBy { get; set; } = "Self";

        // -------------------------------------------------------------
        // SECTION 2: About Me & Partner Expectations
        // -------------------------------------------------------------
        [MaxLength(1000)]
        public string? AboutMe { get; set; }
        
        [MaxLength(500)]
        public string? PartnerExpectations { get; set; }

        // -------------------------------------------------------------
        // SECTION 3: Education
        // -------------------------------------------------------------
        [MaxLength(100)]
        public string? HighestEducation { get; set; }
        
        [MaxLength(150)]
        public string? CollegeOrUniversity { get; set; }
        
        [MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        // -------------------------------------------------------------
        // SECTION 4: Profession & Career
        // -------------------------------------------------------------
        [MaxLength(50)]
        public string? EmployedIn { get; set; } = "Private Sector";
        
        [MaxLength(100)]
        public string? Occupation { get; set; }
        
        [MaxLength(150)]
        public string? CompanyName { get; set; }
        
        [MaxLength(100)]
        public string? WorkLocation { get; set; }

        // -------------------------------------------------------------
        // SECTION 5: Income
        // -------------------------------------------------------------
        [MaxLength(50)]
        public string? AnnualIncome { get; set; } = "₹10 - ₹15 Lakhs";

        // -------------------------------------------------------------
        // SECTION 6: Family Details
        // -------------------------------------------------------------
        [MaxLength(30)]
        public string? FamilyType { get; set; } = "Nuclear";
        
        [MaxLength(30)]
        public string? FamilyValues { get; set; } = "Moderate";
        
        [MaxLength(100)]
        public string? FatherOccupation { get; set; }
        
        [MaxLength(100)]
        public string? MotherOccupation { get; set; }
        
        public int NumberOfBrothers { get; set; } = 0;
        public int NumberOfSisters { get; set; } = 0;
        
        [MaxLength(150)]
        public string? FamilyCity { get; set; }

        // -------------------------------------------------------------
        // SECTION 7: Lifestyle & Habits
        // -------------------------------------------------------------
        [MaxLength(30)]
        public string? Diet { get; set; } = "Vegetarian";
        
        [MaxLength(30)]
        public string? Drink { get; set; } = "No";
        
        [MaxLength(30)]
        public string? Smoke { get; set; } = "No";

        // -------------------------------------------------------------
        // SECTION 8: Hobbies & Interests
        // -------------------------------------------------------------
        [MaxLength(300)]
        public string? Hobbies { get; set; } = "Travelling, Reading, Music, Fitness";

        // -------------------------------------------------------------
        // SECTION 9: Religion & Astrology (Kundali / Horoscope)
        // -------------------------------------------------------------
        [MaxLength(100)]
        public string? SubCasteOrGothra { get; set; }
        
        [MaxLength(20)]
        public string? ManglikStatus { get; set; } = "No";
        
        [MaxLength(50)]
        public string? Rashi { get; set; }
        
        [MaxLength(50)]
        public string? Nakshatra { get; set; }

        // -------------------------------------------------------------
        // SECTION 10: Location & Relocation Preference
        // -------------------------------------------------------------
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(100)]
        public string? State { get; set; }
        
        [MaxLength(100)]
        public string? Country { get; set; } = "India";
        
        [MaxLength(100)]
        public string? NativePlace { get; set; }
        
        public bool WillingToRelocate { get; set; } = true;

        // -------------------------------------------------------------
        // SECTION 11: Photos Gallery & Profile Completeness
        // -------------------------------------------------------------
        public string? PhotoGalleryJson { get; set; }
        public int ProfileCompletionPercentage { get; set; } = 85;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

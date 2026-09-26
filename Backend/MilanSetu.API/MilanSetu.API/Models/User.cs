using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[Users]
    /// Primary authentication, personal account credentials, and basic registration information.
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: Name (NVARCHAR(100), NOT NULL) - Full Name of the candidate
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Column: Gender (NVARCHAR(20), NOT NULL) - 'Male' or 'Female'
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Column: DateOfBirth (DATETIME2, NOT NULL) - DOB for age calculation
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Column: Email (NVARCHAR(150), Unique Index, NOT NULL) - Login identifier
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Column: Mobile (NVARCHAR(20), Unique Index, NOT NULL) - Contact / OTP identifier
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// Column: PasswordHash (NVARCHAR(MAX), NOT NULL) - BCrypt encrypted password hash
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Column: Religion (NVARCHAR(50), NOT NULL) - Hindu, Muslim, Sikh, Christian, Jain, etc.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Religion { get; set; } = string.Empty;

        /// <summary>
        /// Column: Caste (NVARCHAR(100), NULL) - Brahmin, Maratha, Rajput, Kayastha, etc.
        /// </summary>
        [MaxLength(100)]
        public string? Caste { get; set; }

        /// <summary>
        /// Column: MotherTongue (NVARCHAR(50), NOT NULL) - Hindi, Bengali, Marathi, Tamil, etc.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string MotherTongue { get; set; } = string.Empty;

        /// <summary>
        /// Column: Location (NVARCHAR(150), NOT NULL) - Current City, State, Country
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Column: ProfilePhotoUrl (NVARCHAR(MAX), NULL) - Display Avatar Image
        /// </summary>
        public string? ProfilePhotoUrl { get; set; }

        /// <summary>
        /// Column: IsVerified (BIT, NOT NULL, Default: 1) - Account verification status
        /// </summary>
        public bool IsVerified { get; set; } = true;

        /// <summary>
        /// Column: Role (NVARCHAR(20), NOT NULL, Default: 'User') - 'User' or 'Admin'
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Column: IsBlocked (BIT, NOT NULL, Default: 0) - Admin ban status
        /// </summary>
        public bool IsBlocked { get; set; } = false;

        /// <summary>
        /// Column: PreferredLanguage (NVARCHAR(10), NOT NULL, Default: 'en') - 'en', 'hi', 'bn', 'mr', 'ta', 'te', 'gu', 'kn'
        /// </summary>
        [MaxLength(10)]
        public string PreferredLanguage { get; set; } = "en";

        /// <summary>
        /// Column: CreatedAt (DATETIME2, NOT NULL) - Registration timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties: Relates to dbo.UserProfiles and dbo.PartnerPreferences
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
        public virtual ICollection<PartnerPreference> PartnerPreferences { get; set; } = new List<PartnerPreference>();
    }
}

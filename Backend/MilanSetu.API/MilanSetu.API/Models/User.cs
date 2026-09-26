using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Religion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Caste { get; set; }

        [Required]
        [MaxLength(50)]
        public string MotherTongue { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        public string? ProfilePhotoUrl { get; set; }

        public bool IsVerified { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace MilanSetu.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Mobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Religion is required")]
        public string Religion { get; set; } = string.Empty;

        public string? Caste { get; set; }

        [Required(ErrorMessage = "Mother Tongue is required")]
        public string MotherTongue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string? Caste { get; set; }
        public string MotherTongue { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

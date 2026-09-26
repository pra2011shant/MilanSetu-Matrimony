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
        [Required(ErrorMessage = "Email or Mobile number is required")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email or Mobile number is required")]
        public string Identifier { get; set; } = string.Empty;
    }

    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email or Mobile number is required")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "6-digit OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        public string OtpCode { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email or Mobile number is required")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "6-digit OTP code is required")]
        [StringLength(6, MinimumLength = 6)]
        public string OtpCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserResponseDto User { get; set; } = null!;
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
        public string PreferredLanguage { get; set; } = "en";
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateLanguageDto
    {
        [Required]
        [MaxLength(10)]
        public string Language { get; set; } = "en";
    }

    public class GoogleLoginDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        public string? GoogleId { get; set; }

        public string? Token { get; set; }
    }
}

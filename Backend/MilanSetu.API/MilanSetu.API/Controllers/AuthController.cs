using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using MilanSetu.API.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                return BadRequest(new { message = "An account with this email address already exists." });
            }

            // Check if mobile already exists
            var mobileExists = await _context.Users.AnyAsync(u => u.Mobile == dto.Mobile);
            if (mobileExists)
            {
                return BadRequest(new { message = "An account with this mobile number already exists." });
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Default profile photo based on gender
            var defaultAvatar = dto.Gender.Equals("Bride", StringComparison.OrdinalIgnoreCase) || dto.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase)
                ? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80"
                : "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80";

            var user = new User
            {
                Name = dto.Name.Trim(),
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email.Trim().ToLower(),
                Mobile = dto.Mobile.Trim(),
                PasswordHash = passwordHash,
                Religion = dto.Religion,
                Caste = dto.Caste?.Trim(),
                MotherTongue = dto.MotherTongue,
                Location = dto.Location.Trim(),
                ProfilePhotoUrl = defaultAvatar,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);
            var age = CalculateAge(user.DateOfBirth);

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                Email = user.Email,
                Mobile = user.Mobile,
                Religion = user.Religion,
                Caste = user.Caste,
                MotherTongue = user.MotherTongue,
                Location = user.Location,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                Message = "Registration successful! Welcome to MilanSetu.",
                User = userResponse
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identifier = dto.Identifier.Trim().ToLower();

            // Match by either Email or Mobile Number
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == identifier || u.Mobile == identifier);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email/mobile or password." });
            }

            var token = _jwtService.GenerateToken(user);
            var age = CalculateAge(user.DateOfBirth);

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                Email = user.Email,
                Mobile = user.Mobile,
                Religion = user.Religion,
                Caste = user.Caste,
                MotherTongue = user.MotherTongue,
                Location = user.Location,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            };

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                Message = "Login successful! Welcome back.",
                User = userResponse
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identifier = dto.Identifier.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == identifier || u.Mobile == identifier);

            if (user == null)
            {
                return NotFound(new { message = "No registered account found with this email or mobile number." });
            }

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Invalidate existing unused OTPs
            var existingOtps = await _context.PasswordResetOtps
                .Where(o => o.Identifier == identifier && !o.IsUsed)
                .ToListAsync();

            foreach (var o in existingOtps)
            {
                o.IsUsed = true;
            }

            var resetOtp = new PasswordResetOtp
            {
                Identifier = identifier,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResetOtps.Add(resetOtp);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Verification OTP has been sent successfully to your registered { (identifier.Contains("@") ? "email" : "mobile") }.",
                identifier = identifier,
                otpPreview = otp, // Included for seamless testing & quick verification
                expiresInMinutes = 10
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identifier = dto.Identifier.Trim().ToLower();

            var otpRecord = await _context.PasswordResetOtps
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(o => o.Identifier == identifier && o.OtpCode == dto.OtpCode && !o.IsUsed);

            if (otpRecord == null)
            {
                return BadRequest(new { message = "Invalid OTP entered. Please verify." });
            }

            if (otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { message = "The OTP has expired. Please request a new one." });
            }

            return Ok(new { message = "OTP verified successfully. You may now reset your password." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identifier = dto.Identifier.Trim().ToLower();

            var otpRecord = await _context.PasswordResetOtps
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(o => o.Identifier == identifier && o.OtpCode == dto.OtpCode && !o.IsUsed);

            if (otpRecord == null || otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired OTP. Please request a new verification code." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == identifier || u.Mobile == identifier);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Update user password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            otpRecord.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been successfully updated! You can now log in with your new password." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            var age = CalculateAge(user.DateOfBirth);

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                Email = user.Email,
                Mobile = user.Mobile,
                Religion = user.Religion,
                Caste = user.Caste,
                MotherTongue = user.MotherTongue,
                Location = user.Location,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var age = CalculateAge(user.DateOfBirth);

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                Email = user.Email,
                Mobile = user.Mobile,
                Religion = user.Religion,
                Caste = user.Caste,
                MotherTongue = user.MotherTongue,
                Location = user.Location,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    Age = DateTime.UtcNow.Year - u.DateOfBirth.Year,
                    Email = u.Email,
                    Mobile = u.Mobile,
                    Religion = u.Religion,
                    Caste = u.Caste,
                    MotherTongue = u.MotherTongue,
                    Location = u.Location,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                    IsVerified = u.IsVerified,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        private static int CalculateAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Data;
using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
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

            var age = CalculateAge(user.DateOfBirth);

            var response = new UserResponseDto
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

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new
            {
                message = "Registration successful! Welcome to MilanSetu.",
                user = response
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var age = CalculateAge(user.DateOfBirth);

            var response = new UserResponseDto
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

            return Ok(new
            {
                message = "Login successful!",
                user = response
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

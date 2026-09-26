using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MilanSetu.API.Models;

namespace MilanSetu.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var keyString = _configuration["Jwt:Key"] ?? "MilanSetuSuperSecretKeyForMatrimonialPortalJwtTokenAuthentication2026";
            var issuer = _configuration["Jwt:Issuer"] ?? "MilanSetu.API";
            var audience = _configuration["Jwt:Audience"] ?? "MilanSetu.UI";
            var durationInMinutes = double.TryParse(_configuration["Jwt:DurationInMinutes"], out var minutes) ? minutes : 1440;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Gender, user.Gender),
                new Claim("Mobile", user.Mobile),
                new Claim("Religion", user.Religion),
                new Claim("MotherTongue", user.MotherTongue),
                new Claim("Location", user.Location),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

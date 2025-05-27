using System.Security.Claims;
using System.Text;
using FileFlowApi.Models;
using FileFlowApi.IREPOSITORY;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace FileFlowApi.SERVICES
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
              new Claim(ClaimTypes.Name, user.Username),
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(ClaimTypes.Role, user.Role), // ← זה השורה שחסרה
                new Claim(ClaimTypes.Email, user.Email)
            };
            
            var secretKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (secretKey == null)
            {
                Console.WriteLine("JWT Key is null!");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your-app",
                audience: "your-app",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

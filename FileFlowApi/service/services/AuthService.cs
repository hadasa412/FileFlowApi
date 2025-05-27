using core.IServices;
using FileFlowApi.IREPOSITORY;
using FileFlowApi.Models.DTOs;
using FileFlowApi.Models;
using FileFlowApi.SERVICES;

namespace FileFlowApi.Services
{
    public class AuthService : IAuthService // יש לוודא שהמחלקה מיישמת את הממשק
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Username = registerDto.UserName,  // הוספת שם משתמש
                Email = registerDto.Email,  // הוספת אימייל
                PasswordHash = HashPassword(registerDto.Password),
                Role = registerDto.Role ?? "User"
            };

            // במקום להעביר את user ישירות, העבר את ה-RegisterDto
            await _userService.AddUserAsync(registerDto);  // העבר את ה-DTO, לא את ה-User
            return user;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // חיפוש לפי אימייל בלבד, כיוון שאין יותר UserName ב-LoginDto
            var user = await _userService.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null; // אם לא נמצא או שהסיסמה לא תואמת
            }

            // יצירת טוקן אם ההתחברות הצליחה
            return _tokenService.GenerateToken(user);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}

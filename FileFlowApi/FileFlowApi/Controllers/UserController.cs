    using Microsoft.AspNetCore.Mvc;
    using FileFlowApi.Models;
    using FileFlowApi.Services;
    using System.Threading.Tasks;
    using FileFlowApi.Models.DTOs;


    using Org.BouncyCastle.Crypto.Generators;
    using FileFlowApi.IREPOSITORY;
    using FileFlowApi.SERVICES;
    using core.IServices;
using Microsoft.AspNetCore.Authorization;

    namespace FileFlowApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;
            private readonly IAuthService _authService;
            private readonly ITokenService _tokenService;

            public UsersController(IUserService userService, IAuthService authService, ITokenService tokenService)
            {
                _userService = userService;
                _authService = authService;
                _tokenService = tokenService;
            }

        // פעולה להתחברות של משתמש
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid data.");
            }

            User user;

            if (!string.IsNullOrEmpty(loginDto.Email))
            {
                user = await _userService.GetUserByEmailAsync(loginDto.Email);
            }
            else
            {
                return BadRequest("Email must be provided.");
            }

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                Role = user.Role  // ➡️ מחזיר את התפקיד של המשתמש 
            });
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // תן ברירת מחדל לתפקיד של המשתמש
                    registerDto.Role = "User";

                    // הוסף את המשתמש למסד הנתונים
                    await _userService.AddUserAsync(registerDto);

                    // קבל את המשתמש שנוצר
                    var user = await _userService.GetUserByEmailAsync(registerDto.Email);

                    // אם לא נמצא המשתמש, החזר שגיאה
                    if (user == null)
                    {
                        return BadRequest("Error creating user.");
                    }

                    // יצירת הטוקן
                    var token = _tokenService.GenerateToken(user);

                    // החזרת התגובה עם הטוקן
                    return Ok(new
                    {
                        Token = token,
                        Role = user.Role  // מחזיר את התפקיד של המשתמש
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Invalid data");
        }
        private bool VerifyPassword(string password, string storedHash)
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }

        [HttpGet("{userId}/categories")]
        public async Task<IActionResult> GetUserCategories(int userId)
        {
            var categories = await _userService.GetUserCategoriesAsync(userId);
            if (categories == null || categories.Count == 0)
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyAction()
        {
            return Ok("This is an admin-only route.");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}

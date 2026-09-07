using Microsoft.AspNetCore.Mvc;
using SubastaYa.Api.Responses;
using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using System.Threading.Tasks;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email
            };

            var createdUser = await _userService.RegisterAsync(user, request.Password);

            // ocultamos el hash
            createdUser.PasswordHash = string.Empty;

            return Ok(ApiResponse<object>.Ok(createdUser, "Usuario registrado exitosamente."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userService.LoginAsync(request.Email, request.Password);

            return Ok(ApiResponse<object>.Ok(new { Token = token }, "Inicio de sesión exitoso."));
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

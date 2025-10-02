using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Dto;
using Users.Application.DTO;
using Users.Application.Interfaces;

namespace Users.Presentation.API.Controllers
{
    [Route("api/identity")]
    public class IdentityController : Controller
    {
        private readonly IRegistrationService _RegistrationService;
        private readonly IAuthService _authService;
        private readonly EmailSettings _emailSettings;

        public IdentityController(IRegistrationService RegistrationService, IAuthService authService, IUserService userService, EmailSettings emailSettings)
        {
            _RegistrationService = RegistrationService;
            _authService = authService;
            _emailSettings = emailSettings;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto request)
        {
            var response = await _RegistrationService.RegisterAsync(request);

            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ResponseDto> Login([FromBody] UserLoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            return response;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ResponseDto> Logout()
        {
            var response = await _authService.LogoutAsync();

            return response;
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            await _RegistrationService.ConfirmEmailAsync(token, email);

            return Redirect($"{_emailSettings.FrontUrl}/login?message=Your email is verify. Welcom");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<bool> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            await _authService.ForgotPasswordAsync(forgotPasswordRequestDto.Email);
            return true;
        }

        [HttpPost("reset-password")]
        public async Task<ResponseDto> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            await _authService.ResetPasswordAsync(resetPasswordDto);

            return new ResponseDto();
        }
    }
}

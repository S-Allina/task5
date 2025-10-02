using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Users.Application.Dto;
using Users.Application.Interfaces;
using Users.Domain.Entity;
using Users.Domain.Enums;

namespace Users.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private const string purpose = "ResetPassword";
        private const string separator = ", ";

        public AuthService(IEmailService emailService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMapper mapper, SignInManager<ApplicationUser> signInManager)
        {
            _emailService = emailService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
        }

        public async Task<ResponseDto> LoginAsync(UserLoginRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await AuthenticateUserAsync(request);
            var result = new ResponseDto();

            await _signInManager.SignInAsync(user, isPersistent: false);

            var userResponse = _mapper.Map<UserDto>(user);

            userResponse.LastActivity = DateTime.Now;

            result.Result = userResponse;

            result.ErrorMessages ??= new List<string>();

            if (user?.Status == Statuses.Blocked) {
                result.ErrorMessages.Add("You have been blocked.");
                result.IsSuccess = false;
            }

            if(user?.Status == Statuses.Unverify) result.DisplayMessage = "You need verify your email.";
            
            return result;
        }

        public async Task<ResponseDto> LogoutAsync()
        {
            var f = _currentUserService.GetUserId();
            await _signInManager.SignOutAsync();

            return new ResponseDto { DisplayMessage = "You logout success." };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) throw new Exception($"Account with email {email} not found");

            if (user.Status == Statuses.Unverify) throw new Exception($"Email {email} not verify");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user!);

            await _emailService.SendPasswordResetEmailAsync(user, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null) throw new Exception("Account not found.");

            await ResetUserPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            return true;
        }

        private async Task ResetUserPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, purpose, token);

            if (!isValidToken) throw new Exception("Invalid token");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)  throw new Exception($"Failed to reset password: {string.Join(separator, result.Errors)}");
        }

        private async Task<ApplicationUser> AuthenticateUserAsync(UserLoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var isValidPassword = await _userManager.CheckPasswordAsync(user!, request.Password);

            if (user == null || !isValidPassword)
            {
                throw new Exception("Invalid email or password. Please try again.");
            }

            return user;
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Users.Application.Dto;
using Users.Application.Interfaces;
using Users.Domain.Entity;
using Users.Domain.Enums;

namespace Users.Infrastructure.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public RegistrationService(UserManager<ApplicationUser> userManager, IMapper mapper, IUserService userService, IEmailService emailService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<ResponseDto> RegisterAsync(UserRegistrationRequestDto requestDto)
        {
            var newUser = await CreateUserAsync(requestDto);

            await _emailService.SendVerificationEmailAsync(newUser);

            await _userManager.AddToRoleAsync(newUser, "Admin");

            return new ResponseDto
            {
                Result = _mapper.Map<UserDto>(newUser),
                DisplayMessage = "Your profile has been successfully created. Please confirm your email address to complete registration."
            };
        }

        public async Task ConfirmEmailAsync(string token, string email)
        {
            await _emailService.ConfirmEmailAsync(token, email);

            var user = await _userManager.FindByEmailAsync(email);

            await _userService.UpdateUsersStatusAsync(new string[] { user?.Id ?? string.Empty }, user => user.Status == Statuses.Blocked ? Statuses.Blocked : Statuses.Activity, new CancellationToken());
        }

        private async Task<ApplicationUser> CreateUserAsync(UserRegistrationRequestDto requestDto)
        {
            
            var newUser = _mapper.Map<ApplicationUser>(requestDto);

            newUser.UserName = newUser.FirstName + newUser.LastName + newUser.Email;
            newUser.LastActivity = DateTime.Now;

            var result = await _userManager.CreateAsync(newUser, requestDto.Password);

            if (!result.Succeeded) {
                var errorDescriptions = result.Errors.Select(error => error.Description);
                throw new Exception($"Failed to create user: {string.Join(", ", errorDescriptions)}");
            }

            return newUser;
        }
    }
}

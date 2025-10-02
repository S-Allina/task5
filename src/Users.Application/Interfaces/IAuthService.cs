using Users.Application.Dto;

namespace Users.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(UserLoginRequestDto request);
        Task<ResponseDto> LogoutAsync();
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}

using Users.Application.Dto;

namespace Users.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<ResponseDto> RegisterAsync(UserRegistrationRequestDto requestDto);
        Task ConfirmEmailAsync(string token, string email);
    }
}

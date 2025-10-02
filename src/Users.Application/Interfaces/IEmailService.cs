using Users.Domain.Entity;

namespace Users.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> ConfirmEmailAsync(string token, string email);
        Task SendVerificationEmailAsync(ApplicationUser user);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string token);
    }
}

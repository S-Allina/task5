using Users.Domain.Entity;

namespace Users.Application.Interfaces
{
    public interface ICurrentUserService
    {
        public string? GetUserId();
        Task<ApplicationUser> GetCurrentUserAsync();
    }
}

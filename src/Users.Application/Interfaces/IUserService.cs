using Users.Application.Dto;
using Users.Domain.Entity;
using Users.Domain.Enums;

namespace Users.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken);
        Task<ResponseDto> UpdateUsersStatusAsync(IEnumerable<string> userIds, Func<ApplicationUser, Statuses> statusSelector, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateLastActivity(CancellationToken cancellationToken);
        Task<ResponseDto> BlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> UnlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken);
    }
}

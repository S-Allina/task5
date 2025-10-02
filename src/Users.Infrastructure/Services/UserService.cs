using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Users.Application.Dto;
using Users.Application.Interfaces;
using Users.Domain.Entity;
using Users.Domain.Enums;

namespace Users.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseDto> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.Where(u => !u.EmailConfirmed).Select(u => u.Id).ToListAsync(cancellationToken);

            return await DeleteUsersAsync(users, cancellationToken);
        }

        public async Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds == null) return await GetAllAsync(cancellationToken);

            return await DeleteUsersAsync(userIds, cancellationToken);
        }

        public async Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var user = await GetUserByIdAsync(id, cancellationToken);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);

            return new ResponseDto { Result = _mapper.Map<IEnumerable<UserDto>>(users) };
        }

        public async Task<ResponseDto> BlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            return await UpdateUsersStatusAsync(userIds, user => Statuses.Blocked, cancellationToken);
        }

        public async Task<ResponseDto> UnlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            return await UpdateUsersStatusAsync(userIds, user =>
                user.EmailConfirmed ? Statuses.Activity : Statuses.Unverify, cancellationToken);
        }

        public async Task<ResponseDto> UpdateUsersStatusAsync(IEnumerable<string> userIds, Func<ApplicationUser, Statuses> statusSelector, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var usersToUpdate = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);

            foreach (var user in usersToUpdate)
            {
                user.Status = statusSelector(user);

                await _userManager.UpdateAsync(user);
            }

            return await GetAllAsync(cancellationToken);
        }

        public async Task<ResponseDto> UpdateLastActivity(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _currentUserService.GetCurrentUserAsync();

            if (user != null)
            {
                user.LastActivity = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }

            return new ResponseDto { Result = user };
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) throw new Exception("User not found");

            return user;
        }

        private async Task<ResponseDto> DeleteUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var tasks = _userManager.Users.Where(u => userIds.Contains(u.Id)).ExecuteDeleteAsync(cancellationToken);
            await Task.WhenAll(tasks);

            return await GetAllAsync(cancellationToken);
        }
    }
}

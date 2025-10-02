using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Dto;
using Users.Application.Interfaces;

namespace Users.Presentation.API.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IRegistrationService RegistrationService, IAuthService authService, IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch("block")]
        [Authorize]
        public async Task<ResponseDto> Block([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.BlockUser(userIds, cancellationToken);
        }

        [HttpPatch("unblock")]
        [Authorize]
        public async Task<ResponseDto> Unblock([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.UnlockUser(userIds, cancellationToken);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDto> Get(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        [HttpDelete("unconfirmedUsers")]
        [Authorize]
        public async Task<ResponseDto> DeleteUnconfirmedUsers(CancellationToken cancellationToken)
        {
            return await _userService.DeleteUnconfirmedUsersAsync(cancellationToken);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ResponseDto> DeleteUsers([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.DeleteSomeUsersAsync(userIds, cancellationToken);
        }

        [HttpPost("activity")]
        [Authorize]
        public async Task<IActionResult> UpdateActivity(CancellationToken cancellationToken)
        {
            await _userService.UpdateLastActivity(cancellationToken);

            return Ok();
        }
    }
}

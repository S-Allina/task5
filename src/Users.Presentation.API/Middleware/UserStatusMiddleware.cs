using Users.Application.Interfaces;
using Users.Domain.Enums;

namespace Users.Presentation.API.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService, ICurrentUserService currentUserService)
        {
            if (context.Request.Path.StartsWithSegments("/Login"))
            {
                await _next(context);
                return;
            }

            var userId = currentUserService.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var userResponse = await userService.GetByIdAsync(userId, new CancellationToken());

                if (userResponse == null || userResponse.Status == Statuses.Blocked)
                {
                    var statusMessage = userResponse == null ? "deleted" : userResponse?.Status?.ToString().ToLower();
                    context.Response.Redirect($"/Login?message=Your account has been {statusMessage}");
                    return;
                }
            }

            await _next(context);
        }
    }
}

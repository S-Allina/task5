using Users.Domain.Enums;

namespace Users.Application.Dto
{
    public record UserUpdateRequestDto
    {
        public required string Id { get; set; }
        public required Statuses Status { get; set; }
    }
}

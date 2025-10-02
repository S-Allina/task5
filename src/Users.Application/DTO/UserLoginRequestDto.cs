namespace Users.Application.Dto
{
    public record UserLoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}

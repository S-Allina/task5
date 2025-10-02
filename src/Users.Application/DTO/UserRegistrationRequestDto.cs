namespace Users.Application.Dto
{
    public record UserRegistrationRequestDto
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Job { get; init; } = string.Empty;
    }
}

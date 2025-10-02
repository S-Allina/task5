namespace Users.Application.DTO
{
    public record ForgotPasswordRequestDto
    {
        public string Email { get; init; } = string.Empty;
    }
}

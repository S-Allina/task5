namespace Users.Application.Dto
{
    public record ResetPasswordDto
    {
        public required string Email { get; init; }
        public required string NewPassword { get; init; }
        public required string Token { get; init; }
    }
}

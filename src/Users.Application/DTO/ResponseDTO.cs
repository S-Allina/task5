namespace Users.Application.Dto
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object? Result { get; set; } = default;
        public string DisplayMessage { get; set; } = string.Empty;
        public List<string>? ErrorMessages { get; set; } = new List<string>();
    }
}

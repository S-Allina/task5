namespace Users.Application.DTO
{
    public class EmailSettings
    {
        public string From { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public string Port { get; set; }= string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FrontUrl {  get; set; } = string.Empty;
        public string BackUrl {  get; set; } = string.Empty;
    }
}

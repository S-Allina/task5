using System.Text.Json.Serialization;
using Users.Domain.Enums;

namespace Users.Application.Dto
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public string Job { get; set; } = string.Empty;
        public DateTime LastActivity { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses? Status { get; set; } = Statuses.Unverify;
    }
}

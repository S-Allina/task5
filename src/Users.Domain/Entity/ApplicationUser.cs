using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Users.Domain.Enums;

namespace Users.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Job { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses Status { get; set; } = Statuses.Unverify;
        public DateTime? LastActivity { get; set; }
    }
}

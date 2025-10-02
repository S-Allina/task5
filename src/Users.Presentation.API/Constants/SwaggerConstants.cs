using Microsoft.OpenApi.Models;

namespace Users.Presentation.Constants
{
    public static class SwaggerConstants
    {
        public const string Scheme = "Bearer";
        public const string Name = "Authorization";
        public const SecuritySchemeType Type = SecuritySchemeType.Http;
        public const string BearerFormat = "JWT";
        public const ParameterLocation In = ParameterLocation.Header;
        public const string Description = "JWT Authorization header using the Bearer scheme.";
        public const ReferenceType TypeReference = ReferenceType.SecurityScheme;
        public const string JwtSettingsSection = "JwtSettings";
        public const string SwaggerVersion = "v1";
    }
}

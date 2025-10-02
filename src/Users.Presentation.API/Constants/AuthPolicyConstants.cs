namespace Users.Presentation.Constants
{
    public static class AuthPolicyConstants
    {
        public const int PasswordLength = 8;
        public const int MaxFailedAttempts = 3;
        public const int LockoutMinutes = 5;
        public const string GoogleCallbackPath = "/signin-google";
        public const string Title = "User Auth";
        public const string Description = "Services to Authenticate user";
        public const string ClientID = "Auth:Google:ClientID";
        public const string ClientSecret = "Auth:Google:ClientSecret";
    }
}

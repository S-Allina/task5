using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Users.Application.DTO;
using Users.Application.Interfaces;
using Users.Domain.Entity;

namespace Users.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSettings _emailSettings;

        public EmailService(UserManager<ApplicationUser> userManager, EmailSettings emailSettings)
        {
            _userManager = userManager;
            _emailSettings = emailSettings;
            
        }

        public async Task SendPasswordResetEmailAsync(ApplicationUser user, string token)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var resetUrl = GenerateVerificationUrl(user.Email ?? "", token, $"{_emailSettings.FrontUrl}/reset-password");

            var emailBody = $"Please reset your password by clicking here: <a href='{resetUrl}'>link</a>";

            await SendEmailAsync(user.Email ?? "", "Password Reset Request", emailBody);
        }

        public async Task SendVerificationEmailAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var verificationUrl = GenerateVerificationUrl(user.Email ?? "", token, $"{_emailSettings.BackUrl}/api/identity/verify-email");

            var emailBody = $"Please verify your email by clicking here: <a href='{verificationUrl}'>link</a>";

            await SendEmailAsync(user.Email ?? "", "Verify Your Email", emailBody);
        }

        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) throw new Exception("Invalid email");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded) throw new Exception($"Error confirming email: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return result.Succeeded;
        }


        private async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MailMessage(_emailSettings.From, email, subject, body);
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_emailSettings.SmtpServer, int.Parse(_emailSettings.Port))
            {
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        private string GenerateVerificationUrl(string email, string token, string path)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(email);

            return $"{path}?token={encodedToken}&email={encodedEmail}";
        }
    }
}

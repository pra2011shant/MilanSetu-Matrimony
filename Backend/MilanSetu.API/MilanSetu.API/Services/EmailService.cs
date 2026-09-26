using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MilanSetu.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otpCode)
        {
            var subject = "🔐 MilanSetu - Your Password Reset Verification Code";
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #fff1f2; margin: 0; padding: 20px; }}
        .email-container {{ max-width: 550px; margin: 0 auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.08); border: 1px solid #ffe4e6; }}
        .header {{ background: linear-gradient(135deg, #e11d48, #be123c); padding: 30px 20px; text-align: center; color: #ffffff; }}
        .header h1 {{ margin: 0; font-size: 26px; font-weight: 700; letter-spacing: 0.5px; }}
        .header p {{ margin: 5px 0 0; opacity: 0.9; font-size: 14px; }}
        .content {{ padding: 30px; color: #334155; line-height: 1.6; }}
        .otp-box {{ text-align: center; margin: 25px 0; background: #fdf2f8; border: 2px dashed #f43f5e; border-radius: 12px; padding: 20px; }}
        .otp-code {{ font-size: 36px; font-weight: 800; color: #e11d48; letter-spacing: 6px; }}
        .footer {{ background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #94a3b8; border-top: 1px solid #f1f5f9; }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>💍 MilanSetu Matrimony</h1>
            <p>Trusted Matchmaking & Life Partner Portal</p>
        </div>
        <div class='content'>
            <h2>Hello {WebUtility.HtmlEncode(userName ?? "User")},</h2>
            <p>We received a request to reset your password for your MilanSetu Matrimony account.</p>
            <p>Please use the following 6-digit Verification Code (OTP) to complete your password reset. This code is valid for <strong>10 minutes</strong>.</p>
            
            <div class='otp-box'>
                <div style='font-size: 13px; color: #64748b; margin-bottom: 6px; text-transform: uppercase; font-weight: 600;'>Your One-Time Password</div>
                <div class='otp-code'>{otpCode}</div>
                <div style='font-size: 12px; color: #94a3b8; margin-top: 6px;'>Valid for 10 minutes • Do not share with anyone</div>
            </div>

            <p style='font-size: 13px; color: #64748b;'>If you did not request this password reset, please ignore this email or contact support immediately.</p>
        </div>
        <div class='footer'>
            &copy; 2026 MilanSetu Matrimony Platform. All rights reserved.
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "🎉 Welcome to MilanSetu - Find Your Perfect Life Partner";
            var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #fdf2f8; padding: 20px;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 25px; border-radius: 12px; border: 1px solid #fbcfe8;'>
        <h2 style='color: #e11d48;'>Welcome to MilanSetu, {WebUtility.HtmlEncode(userName)}! 💍</h2>
        <p>Your matrimonial profile has been created successfully.</p>
        <p>You can now search 100% verified profiles, express interest, and connect with prospective life partners.</p>
        <p style='color: #64748b; font-size: 13px;'>Best wishes,<br>The MilanSetu Matchmaking Team</p>
    </div>
</body>
</html>";
            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendProfileViewNotificationEmailAsync(string toEmail, string recipientName, string viewerName, int viewerId)
        {
            var subject = $"👀 {viewerName} (ID: MS-{viewerId:D5}) viewed your MilanSetu Profile!";
            var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #fdf2f8; padding: 20px;'>
    <div style='max-width: 500px; margin: auto; background: white; padding: 25px; border-radius: 12px; border: 1px solid #fbcfe8;'>
        <h2 style='color: #e11d48;'>New Profile Visitor Alert! 👀</h2>
        <p>Dear {WebUtility.HtmlEncode(recipientName)},</p>
        <p><strong>{WebUtility.HtmlEncode(viewerName)}</strong> (Matrimony ID: <strong>MS-{viewerId:D5}</strong>) recently viewed your profile on MilanSetu.</p>
        <p>Log in to your account to view their full matrimonial profile, check compatibility score, or send an Express Interest!</p>
    </div>
</body>
</html>";
            return await SendEmailAsync(toEmail, subject, body);
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
                var smtpPortStr = _config["Smtp:Port"] ?? "587";
                var smtpUser = _config["Smtp:User"];
                var smtpPass = _config["Smtp:Password"];
                var fromAddress = _config["Smtp:FromAddress"] ?? "no-reply@milansetu.com";
                var fromName = _config["Smtp:FromName"] ?? "MilanSetu Matrimony";

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    // Simulated SMTP mode for development / preview
                    _logger.LogInformation($"[Simulated Email] Sent to: {toEmail} | Subject: {subject}");
                    return true;
                }

                int.TryParse(smtpPortStr, out var smtpPort);
                if (smtpPort == 0) smtpPort = 587;

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"[EmailService] Email successfully delivered to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[EmailService] Failed to send email to {toEmail}");
                return false;
            }
        }
    }
}

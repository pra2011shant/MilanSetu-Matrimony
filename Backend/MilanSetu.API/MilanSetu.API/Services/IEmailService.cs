using System.Threading.Tasks;

namespace MilanSetu.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otpCode);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendProfileViewNotificationEmailAsync(string toEmail, string recipientName, string viewerName, int viewerId);
    }
}

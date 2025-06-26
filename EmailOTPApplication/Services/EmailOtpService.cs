using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using EmailOTPApplication.Utilities;

namespace EmailOTPApplication.Services
{
    public class EmailOtpService : IEmailOtpService
    {
        private readonly Dictionary<string, (string otp, DateTime expiry)> _cache = new();
        private readonly Regex _emailReg = new("^[\\w\\.-]+@[\\w\\.-]+\\.dso\\.org\\.sg$", RegexOptions.IgnoreCase);
        private readonly Random _random = new();
        private readonly IEmailSender _emailSender;

        public EmailOtpService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public string GenerateOtpEmail(string email)
        {
            if (!_emailReg.IsMatch(email))
                return EmailStatusCodes.STATUS_EMAIL_INVALID;

            var otp = _random.Next(100000, 999999).ToString();
            _cache[email] = (otp, DateTime.UtcNow.AddMinutes(1));

            string body = $"Your OTP Code is {otp}. The code is valid for 1 minute";

            bool sent = _emailSender.SendEmail(email, body);
            return sent ? EmailStatusCodes.STATUS_EMAIL_OK : EmailStatusCodes.STATUS_EMAIL_FAIL;
        }

        public string CheckOtp(string email, string userInput)
        {
            if (!_cache.ContainsKey(email)) return EmailStatusCodes.STATUS_OTP_FAIL;

            var (otp, expiry) = _cache[email];
            if (DateTime.UtcNow > expiry) return EmailStatusCodes.STATUS_OTP_TIMEOUT;
            if (userInput == otp) return EmailStatusCodes.STATUS_OTP_OK;

            return EmailStatusCodes.STATUS_OTP_FAIL;
        }

      

    }
}
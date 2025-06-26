using System.Net.Mail;
using System.Net;

namespace EmailOTPApplication.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public bool SendEmail(string email, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_config["Smtp:Host"])
                {
                    Port = int.Parse(_config["Smtp:Port"]),
                    Credentials = new NetworkCredential(
                        _config["Smtp:Username"],
                        _config["Smtp:Password"]),
                    EnableSsl = bool.Parse(_config["Smtp:EnableSsl"])
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:FromAddress"], _config["Smtp:FromName"]),
                    Subject = "Your One-Time Password (OTP)",
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
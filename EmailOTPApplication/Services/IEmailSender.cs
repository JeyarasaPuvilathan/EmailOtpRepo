namespace EmailOTPApplication.Services
{
    public interface IEmailSender
    {
        bool SendEmail(string email, string body);
    }
}

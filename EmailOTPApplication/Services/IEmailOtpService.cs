namespace EmailOTPApplication.Services
{
    public interface IEmailOtpService
    {
        string GenerateOtpEmail(string email);
        string CheckOtp(string email, string userInput);
    }
}

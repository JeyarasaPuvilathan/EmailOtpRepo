using EmailOTPApplication.Models;
using EmailOTPApplication.Services;
using EmailOTPApplication.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EmailOTPApplication.Controllers
{
    public class EmailController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        private readonly IEmailOtpService _otpService;
        private static int _attempts = 0;

        public EmailController(IEmailOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpGet]
        public IActionResult RequestOtp() => View();

        [HttpPost]
        public IActionResult RequestOtp(EmailOtp model)
        {
            var result = _otpService.GenerateOtpEmail(model.Email);
            TempData["Email"] = model.Email;
            TempData["Message"] = result;
            if (result == EmailStatusCodes.STATUS_EMAIL_OK)
                return RedirectToAction("VerifyOtp");

            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            ViewBag.Email = TempData["Email"];
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(EmailOtp model)
        {
            _attempts++;
            var result = _otpService.CheckOtp(model.Email, model.OtpCode);

            if (result == EmailStatusCodes.STATUS_OTP_OK)
            {
                _attempts = 0;
                ViewBag.Message = "OTP Verified Successfully!";
            }
            else if (result == EmailStatusCodes.STATUS_OTP_TIMEOUT)
            {
                ViewBag.Message = "OTP Expired.";
            }
            else if (_attempts >= 10)
            {
                ViewBag.Message = "OTP Failed after 10 attempts.";
                _attempts = 0;
            }
            else
            {
                ViewBag.Message = "Invalid OTP. Try again.";
            }

            return View(model);
        }
    }
}


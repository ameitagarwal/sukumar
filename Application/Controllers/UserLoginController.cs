using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Controllers
{
	public class UserLoginController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IStringLocalizer<UserLoginController> _stringLocalizer;
        private readonly IRegisterUserService _registrationService;
        private readonly ILogger<UserLoginController> _logger;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public UserLoginController(UserManager<ApplicationUser> userManager,
				IStringLocalizer<UserLoginController> stringLocalizer,
                IRegisterUserService registrationService,
                ILogger<UserLoginController> logger,
				SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_stringLocalizer = stringLocalizer;
            _registrationService = registrationService;
            _logger = logger;
			_signInManager = signInManager;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> UserLogin(string username, string password)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Find the user in Database
					var user = await _userManager.FindByNameAsync(username);
					if (user != null)
					{
						// Here, you can authenticate the user using your preferred method,
						// such as checking the password hash, using SignInManager, etc.
						// For example, if you're using password-based authentication:

						var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);

						if (signInResult.Succeeded)
						{
                            // User successfully authenticated
                            // Redirect or perform the necessary action
                            return Json(new { status = 0, statusText = "success" });
                        }
						else if (signInResult.IsLockedOut)
						{
							// Account is locked out due to too many failed login attempts
							return Json(new { status = 1, statusText = "Account locked out due to too many failed login attempts. Try again later." });

						}
						else
						{
							// Login failed 
							return Json(new { status = 1, statusText = "Login failed. Please check your credentials and try again." });

						}
					}
					else
					{
						// User with the provided username does not exist
						// Handle accordingly (e.g., show an error message)
						return Json(new { status = 2, statusText = "User Not Exist." });
					}
				}
				else { return Json(new { status = 1, statusText = "Model not valid." }); }
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
				return Json(new { status = 1, statusText = ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> EmiratesID_Login(string EmiratesID)
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Find user based on EmiratesID in database
					var user = await _userManager.Users
					.Where(u => u.EmiratesID == EmiratesID)
					.FirstOrDefaultAsync();

					if (user != null)
					{
						// Sign in the user
						await _signInManager.SignInAsync(user, isPersistent: false);
						// Check particular user authendicated
						bool isAuthenticated = User.Identity.IsAuthenticated;

						if (isAuthenticated)
						{
							return Json(new { status = 0, statusText = "success" });
						}
						else
						{
							return Json(new { status = 1, statusText = "Login failed due to some reason." });
						}
					}
					else
					{
						return Json(new { status = 1, statusText = "EmiratesID Not Registered." });
					}

				}
				else { return Json(new { status = 1, statusText = "Model not valid." }); }
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
				return Json(new { status = 1, statusText = ex.Message });
			}

		}

        [HttpGet]
        public string? ValidateOTP(string EmiratesID)
        {
            try
            {
                UserOTP storedOTP = _registrationService.GetOTPByEmiratesID(EmiratesID);
                if (storedOTP != null)
                {
                    return storedOTP.OTP.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return null;
        }

        [HttpGet]
        public string RegisterOTP(string EmiratesID, string Mobile, string ArabicName)
        {
            try
            {
                //Generate OTP Number
                Mobile = "+971508387544";
                string randomNumber;
                Random rnd = new Random();
                randomNumber = (rnd.Next(100000, 999999)).ToString();

                //Create new transaction in OTP table
                UserOTP Item = new UserOTP()
                {
                    OTP = Convert.ToInt32(randomNumber),
                    Status = 0,
                    EmiratesID = EmiratesID

                };
                UserOTP storedOTP = _registrationService.GetOTPByEmiratesID(EmiratesID);
                if (storedOTP == null)
                {
                    _registrationService.Create(Item);
                }
                else
                {
                    storedOTP.OTP = Item.OTP; // Update the OTP value
                    _registrationService.Update(storedOTP);
                }

                //Send OTP number to SMS User mobile number
                var Message = "use otp: " + randomNumber;
                //_commonService.SendSMS(Message, Mobile);

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return "Suceess";
        }
    }
}

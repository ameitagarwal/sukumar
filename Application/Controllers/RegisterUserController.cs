using System.Reflection;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

//using OtpNet;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;
using TMS_Traning_Management.Security;
using TMS_Traning_Management.ViewModels;

namespace TMS_Traning_Management.Controllers
{
	public class RegisterUserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IStringLocalizer<RegisterUserController> _stringLocalizer;
		private readonly IRegisterUserService _registrationService;
		private readonly ICommonService _commonService;
		private readonly IConfiguration _config;
		private readonly ILogger<RegisterUserController> _logger;
		private readonly IDataProtector protector;
		public RegisterUserController(UserManager<ApplicationUser> userManager,
			IStringLocalizer<RegisterUserController> stringLocalizer,
			IRegisterUserService registrationService,
			ICommonService commonService,
			IConfiguration config,
			ILogger<RegisterUserController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_userManager = userManager;
			_stringLocalizer = stringLocalizer;
			_registrationService = registrationService;
			_commonService = commonService;
			_config = config;
			_logger = logger;
			protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.RegistrationNoRoutValue);
		}
		[HttpGet]
		public IActionResult Index()
		{
			RegisterUserViewModel model = new RegisterUserViewModel();
			try
			{
				model = new RegisterUserViewModel()
				{
					EmiratesID = "",
					EnglishName = "",
					ArabicName = "",
					Address = "",
					Mobile = "",
					Email = "",
					Gender = "",
					DOB = "",
					PreferredLang = "",
					Countrys = _registrationService.GetCountry(),
					SecurityQuestions = _registrationService.GetSecurityQuestion()
				};
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			string host = Request.Headers["Referer"].ToString();
			ViewData["BaseUrl"] = host;
			return View(model);
		}
		[HttpGet]
		public async Task<dynamic> GetEmiratesIdDetails(string emiratesId)
		{
			try
			{
				var emiratesIDs = await _userManager.Users
					.Where(u => u.EmiratesID == emiratesId)
					.Select(u => u.EmiratesID)
					.ToListAsync();
				if (emiratesIDs.Any())
				{
					return Json(new { status = 2, statusText = "Driver Already Registered" });
				}
				else
				{
					var result = await _commonService.GetEmiratesIdDetailsAsync(emiratesId.Replace("-", ""));
					if (result != null)
					{
						if (!((GenericServices.FetchPersonAccountSAResponse2)result).PersonDetails.PersonID.IsNullOrEmpty())
						{
							string ArabicName = ((GenericServices.FetchPersonAccountSAResponse2)result).PersonDetails.ArabicName;
							string Mobile = ((GenericServices.FetchPersonAccountSAResponse2)result).PersonDetails.MobileNumber;
							RegisterOTP(emiratesId, "+971508387544", ArabicName);
							return result;
						}
						else
						{
							return Json(new { status = 1, statusText = "EmiratesID not registered in CC&B" });
						}
					}
					else
					{
						return Json(new { status = 1, statusText = "EmiratesID not registered in CC&B" });
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
				return Json(new { status = 0, statusText = "It seems that something went wrong. Please try again later." });
			}
		}
		[HttpPost]
		public async Task<IActionResult> RegisterUser(RegisterUser registerdata, bool isEidexist)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Add the user in Database
					ApplicationUser user = new()
					{
						EmiratesID = registerdata.EmiratesID,
						Email = registerdata.Email,
						EnglishName = registerdata.EnglishName,
						ArabicName = registerdata.ArabicName,
						Mobile = registerdata.Mobile,
						Gender = registerdata.Gender,
						PreferredLang = registerdata.PreferredLang,
						Address = registerdata.Address,
						//Nationality = registerdata.Nationality,
						DOB = Convert.ToDateTime(registerdata.DOB),
						SecurityStamp = Guid.NewGuid().ToString(),
						UserName = registerdata.UserName
					};

					string ReturnStatus = string.Empty;
					if (!isEidexist)
					{
						string PersonId = GeneratePersonID();
						var respCreatePerson = await _commonService.InsertEmiratesIdDetailsAsync(registerdata.EmiratesID.Replace("-", ""), registerdata.ArabicName, registerdata.EnglishName, registerdata.Email, registerdata.Mobile, registerdata.Address, PersonId);
						ReturnStatus = ((GenericServices.CreateUpdatePersonResponse)respCreatePerson).CreateUpdatePersonResponse1.Status;
						if (ReturnStatus != "success")
						{
							_logger.Log(LogLevel.Error, ReturnStatus);
						}
						else
						{
							var result = await _userManager.CreateAsync(user, registerdata.Password);

							if (!result.Succeeded)
							{
								// Construct the error message using the errors from the result
								string errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));

								return Json(new { status = 0, statusText = errorMessage });
							}
							else
							{


								if (!string.IsNullOrEmpty(registerdata.Email))
								{
									string Body = _stringLocalizer["Registration Completion Email Body"].Value.Replace("{RegNo}", "AD000063");
									registerdata.Email = "sukumar.gajendran@injazat.com";
									var resulrt = _commonService.SendMail(registerdata.Email, _stringLocalizer["Registration Completion Email Subject"].Value, Body);
									if (resulrt != "Success")
									{
										_logger.Log(LogLevel.Error, resulrt);
									}
								}
								return Json(new { status = 1, statusText = "success" });
							}
						}
					}
					else
					{
						var result = await _userManager.CreateAsync(user, registerdata.Password);

						if (!result.Succeeded)
						{
							// Construct the error message using the errors from the result
							string errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));

							return Json(new { status = 0, statusText = errorMessage });
						}
						else
						{


							if (!string.IsNullOrEmpty(registerdata.Email))
							{
								string Body = _stringLocalizer["Registration Completion Email Body"].Value.Replace("{RegNo}", "AD000063");
								registerdata.Email = "sukumar.gajendran@injazat.com";
								var resulrt = _commonService.SendMail(registerdata.Email, _stringLocalizer["Registration Completion Email Subject"].Value, Body);
								if (resulrt != "Success")
								{
									_logger.Log(LogLevel.Error, resulrt);
								}
							}
							return Json(new { status = 1, statusText = "success" });
						}
					}
					//Send registered user information to CC&B
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
				return Json(new { status = 2, statusText = ex.Message });
			}
			return RedirectToAction("Complete", 1);
		}
		[HttpGet]
		public string RegisterOTP(string EmiratesID, string Mobile,string ArabicName)
		{
			try
			{
				string strNameArabic = string.Empty;
				if (ArabicName != null)
				{
					strNameArabic = ArabicName.ToString();
					if (IsArabic(strNameArabic) == true)
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
					else
					{
						return _stringLocalizer["Driver Registration Arabic Name Invalid"];
					}
				}
				else
				{
					return _stringLocalizer["Driver Registration Arabic Name Invalid"];
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return "Suceess";
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
		public IActionResult Complete(string id)
		{
			try
			{

			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return View();
		}
		public string GeneratePersonID()
		{
			Random random = new Random();
			string r = "";
			int i;
			for (i = 1; i < 11; i++)
			{
				r += random.Next(0, 9).ToString();
			}
			return r;
		}

		/// <summary>
		/// Validates if the entered text is in Arabic
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsArabic(String text)
		{
			Boolean IsArabic = false;
			if (text.Length > 0)
			{
				char[] glyphs = text.ToCharArray();

				foreach (char glyph in glyphs)
				{
					//if (glyph >= 0x600 && glyph <= 0x6ff) return true;
					//if (glyph >= 0x750 && glyph <= 0x77f) return true;
					//if (glyph >= 0xfb50 && glyph <= 0xfc3f) return true;
					//if (glyph >= 0xfe70 && glyph <= 0xfefc) return true;
					IsArabic = false;
					if (glyph >= 0x600 && glyph <= 0x6ff)
					{
						IsArabic = true;
					}
					if (glyph >= 0x750 && glyph <= 0x77f)
					{
						IsArabic = true;
					}
					if (glyph >= 0xfb50 && glyph <= 0xfc3f)
					{
						IsArabic = true;
					}
					if (glyph >= 0xfe70 && glyph <= 0xfefc)
					{
						IsArabic = true;
					}
					if (glyph == 32)
					{
						IsArabic = true;
					}

					if (IsArabic == false)
					{
						return IsArabic;
					}

				}
				return IsArabic;
			}
			else
			{
				return false;
			}
		}
	}
}

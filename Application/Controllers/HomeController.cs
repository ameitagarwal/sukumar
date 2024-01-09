using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;
using TMS_Traning_Management.Security;
using TMS_Traning_Management.ViewModels;

namespace TMS_Traning_Management.Controllers
{
	public class HomeController : Controller
	{
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
		private readonly IPaymentService _paymentSèrvice;
		private readonly IRegistrationService _registrationService;
		private readonly ILogger<HomeController> _logger;
		private readonly IDataProtector protector;
		public HomeController(
			IStringLocalizer<HomeController> stringLocalizer,
			IPaymentService paymentService,
			IRegistrationService registrationService,
			ILogger<HomeController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_stringLocalizer = stringLocalizer;
			_paymentSèrvice = paymentService;
			_registrationService = registrationService;
			_logger = logger;
			protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.RegistrationNoRoutValue);
        }
        [HttpGet]
        public IActionResult Index()
		{
			HomePagePaymentViewModel model = new HomePagePaymentViewModel()
			{
				RegistrationNo = "",
				Centers = _registrationService.GetCenters()
			};
            return View(model);
		}
		[HttpPost]
		public IActionResult Index(HomePagePaymentViewModel model)
		{
			if (ModelState.IsValid)
			{
				Registration registration = _paymentSèrvice.GetRegistrationDetails(model.RegistrationNo);
				if (registration != null)
				{
					registration.EncryptedId = protector.Protect(registration.Id.ToString());
					return RedirectToAction("Index", "Payment", new { id = registration.EncryptedId});
				}
				ModelState.AddModelError("RegistrationNo", _stringLocalizer["Home Page Payment Invalid Registration number"]);
            }
			model.Centers = _registrationService.GetCenters();
            return View(model);
        }
		#region Localization
		public IActionResult ChangeLanguage(string culture, string prevculture)
		{
			var url = Request.Headers["Referer"].ToString();
            url = url.Contains(prevculture) ? url.Replace("/" + prevculture, "/" + culture) : url + culture;
			return Redirect(url);
		}
		#endregion
	}
}

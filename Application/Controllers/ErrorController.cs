using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace TMS_Traning_Management.Controllers
{
	public class ErrorController : Controller
	{
		[Route("/Error/{statusCode}")]
		public IActionResult Index(string culture, int statusCode)
		{
			var cul = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			return View("NotFound");
		}
	}
}

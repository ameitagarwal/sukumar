using System.ComponentModel.DataAnnotations;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
	public class RegisterUserViewModel
	{
		[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Registration Email InValid")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Registration Mobile Required")]
		[RegularExpression("^05(?:0|2|3|4|6|7|9)-\\d{7}$", ErrorMessage = "Registration Mobile InValid")]
		public string Mobile { get; set; }

		[Required(ErrorMessage = "Registration EmiratesID Required")]
		[RegularExpression("^784-[0-9]{4}-[0-9]{7}-[0-9]{1}$", ErrorMessage = "Registration EmiratesID InValid")]
		public string EmiratesID { get; set; }

		[Required(ErrorMessage = "Registration Username Required")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Registration Driver Name Required")]

		public string Address { get; set; }

		//public string Nationality { get; set; }

		[Required(ErrorMessage = "Please select an option.")]
		public string Gender { get; set; }
		//public string Gender { get; set; }
		public string DOB { get; set; }

		public string EnglishName { get; set; }

		public string ArabicName { get; set; }

		public string PreferredLang { get; set; }

		public string VerifyOTP { get; set; }

		public string SecurityQuestion { get; set; }
		//
		public string SecurityAnswer { get; set; }

		[Required(ErrorMessage = "Registration Password Required")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Registration Confirm Password Required")]
		[Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$", ErrorMessage = "password must contain at least one uppercase letter, one digit, and be at least 8 characters long")]
		public string ConfirmPassword { get; set; }

		public List<Country>? Countrys { get; set; }

		public List<SecurityQuestion>? SecurityQuestions { get; set; }
    }
}

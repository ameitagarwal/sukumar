using System.Diagnostics.Metrics;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Interfaces
{
	public interface IRegisterUserService
	{
		UserOTP Create(UserOTP model);
		UserOTP GetOTPByEmiratesID(string EmiratesID);
		//RegisterUser GetRegistrationDetailsById(int id);
		UserOTP Update(UserOTP storedOTP);
		List<Country> GetCountry();
		List<SecurityQuestion> GetSecurityQuestion();
	}
}

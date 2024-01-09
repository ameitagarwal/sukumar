using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMS_Traning_Management.Services;

namespace TMS_Traning_Management.Interfaces
{
	public interface ICommonService
	{
		string SendMail(string To, string Subject, string Body);
		Task<string> SendSMS(string message, string mobile);
		Task<dynamic> GetEmiratesIdDetailsAsync(string emirateID);
		Task<dynamic> InsertEmiratesIdDetailsAsync(string Eid, string ArabicName, string EnglishName, string EmailID, string PhoneNumber, string Address, string PersonId);
	}
}

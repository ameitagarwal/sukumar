using System.Net.Mail;
using System.ServiceModel;
using Microsoft.Identity.Client;
using GenericServices;
using Reseon8Service;
using TMS_Traning_Management.Interfaces;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace TMS_Traning_Management.Services
{
	public class CommonService : ICommonService
	{
		private readonly IConfiguration _config;
		public CommonService(IConfiguration config)
		{
			_config = config;
		}
		public string SendMail(string To, string Subject, string Body)
		{
			try
			{
				MailMessage msg = new MailMessage();
				SmtpClient smtp = new SmtpClient(_config["SMTPServerIP"]);
				smtp.Credentials = new System.Net.NetworkCredential(_config["FromAddress"], _config["SMTPServerPassword"]);
				smtp.Port = Convert.ToInt32(_config["SMTPServerPort"]);

				msg.From = new MailAddress(_config["FromAddress"]);
				msg.IsBodyHtml = true;
				msg.To.Add(To);
				msg.Subject = Subject;
				msg.Body = Body;
				smtp.Send(msg);
				return "Success";
			}
			catch(Exception ex)
			{
                return ex.Message;
			}
		}
		public async Task<string> SendSMS(string message, string mobile)
		{
			try
			{
                using (MessageSenderWebServiceClient service = new MessageSenderWebServiceClient())
                {
                    service.Endpoint.Address = new EndpointAddress(_config["Reson8Url"]);
					var sendMessageResponse = await service.sendMessageAsync(
                        _config["Reson8SenderName"].ToString(),
                         _config["Reson8Password"].ToString(),
                        message,
                         _config["Reson8Encoding"].ToString(),
                        GetSMSMobNumber(mobile),
                         _config["Reson8SMSHeader"].ToString(),
                         _config["Reson8CampaignType"].ToString(),
                         _config["Reson8Encrypt"].ToString()
                    );
					return "Success";
                }
            }
			catch (Exception ex)
			{
				throw ex;
			}
		}
        private static string GetSMSMobNumber(string P_MobileNo)
        {
            string SMSMobNumber = "";
            try
            {
                if (P_MobileNo != null)
                {
                    P_MobileNo = P_MobileNo.Replace("-", "");
                    P_MobileNo = P_MobileNo.Replace(" ", "");
                    P_MobileNo = P_MobileNo.Replace("+", "");

                    SMSMobNumber = "00971" + P_MobileNo.Substring((P_MobileNo.Length - 9), 9);

                    return SMSMobNumber;
                }
                else
                {
                    return SMSMobNumber;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }


        }
		public async Task<dynamic> GetEmiratesIdDetailsAsync(string emirateID)
		{
			try
			{
				GenericServices_PortTypeClient.EndpointConfiguration _endpoint = new GenericServices_PortTypeClient.EndpointConfiguration();

				// GenericServices_PortTypeClient _proxy= new GenericServices_PortTypeClient()
				using (GenericServices_PortTypeClient services = new GenericServices_PortTypeClient(_endpoint))
				{

					FetchPersonAccountSARequest request = new FetchPersonAccountSARequest()
					{
						IDType = "EID",
						IDValue = emirateID,
						SAType = "'E-UTRESD','W-UTRESD','E-UTCOMR','W-UTCOMR','E-UTAGRC','W-UTAGRC'",
						SAStatus = "'20'",
						IsPersonDetails = "true",
						IsSADetails = "false",
						IsAccount = "false"
					};

					FetchPersonAccountSAResponse result = await services.FetchPersonAccountSAAsync(request);
					return result.FetchPersonAccountSAResponse1;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<dynamic> InsertEmiratesIdDetailsAsync(string Eid, string ArabicName, string EnglishName, string EmailID, string PhoneNumber, string Address, string PersonId)
		{
			GenericServices_PortTypeClient.EndpointConfiguration _endpoint = new GenericServices_PortTypeClient.EndpointConfiguration();

			using (GenericServices_PortTypeClient services = new GenericServices_PortTypeClient(_endpoint))
			{
				CreateUpdatePersonRequest request = new CreateUpdatePersonRequest()
				{
					EmiratesIDNumber = Eid,
					PersonArabicName = ArabicName,
					PersonEnglishName = EnglishName,
					EmailID = EmailID,
					PhoneNumber = PhoneNumber,
					Address = Address,
					Address2 = "",
					Address3 = "",
					//AddressLine4 = "testline4",
					PersonID = PersonId,
					TradeLicenseNumber = null,
					TransactionType = "ADD"
				};
				CreateUpdatePersonResponse result = await services.CreateUpdatePersonAsync(request);
				return result;
			}
		}


	}
}

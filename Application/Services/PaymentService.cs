using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Comtrust.Payment.IPG.SPInet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;
        public PaymentService(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        public Registration GetRegistrationDetails(string RegistrationNo)
        {
            try
            {
                return _dbContext.Registrations.Where(r => r.RegistrationNo == RegistrationNo).Include(r => r.Training).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Registration GetRegistrationDetailsById(int id)
        {
            try
            {
                return _dbContext.Registrations
                    .Include(r => r.Training)
                    .ThenInclude(t => t.Center)
                    .Include(r => r.Training)
                    .ThenInclude(t => t.Location)
                    .FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Training> GetTrainings(int LocationId, int CenterId)
        {
            try
            {
                return _dbContext.Trainings.Where(t => t.LocationId == LocationId && t.CenterId == CenterId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Registration ChangeTraining(int id, int TrainingId)
        {
            try
            {
                var registration = _dbContext.Registrations.Where(r => r.Id == id).FirstOrDefault();
                //var training = _dbContext.Trainings.Where(t => t.Id == TrainingId).Include(t => t.Location).FirstOrDefault();
                //registration.RegistrationNo = training.Location.Key + id.ToString().PadLeft(6, '0');
                registration.TrainingId = TrainingId;
                _dbContext.Update(registration);
                return registration;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Registration UpdateRegistrationDetails(int id, string PaymentStatus, string TransactionID)
        {
            try
            {
                var registration = _dbContext.Registrations.Where(r => r.Id == id).FirstOrDefault();
                if (PaymentStatus == "I")
                    registration.TTC = DateTime.Now.AddMinutes(5);

                if (registration.PaymentStatus != "I" && PaymentStatus == "I")
                {
                    var training = _dbContext.Trainings.Where(t => t.Id == registration.TrainingId).FirstOrDefault();
                    training.AvailableSeats = training.AvailableSeats - 1;
                }
                if (PaymentStatus == "N")
                {
                    var training = _dbContext.Trainings.Where(t => t.Id == registration.TrainingId).FirstOrDefault();
                    training.AvailableSeats = training.AvailableSeats + 1;
                }
                if (!string.IsNullOrEmpty(TransactionID))
                    registration.TransactionID = TransactionID;

                registration.PaymentStatus = PaymentStatus;
                registration.Updated = DateTime.Now;
                _dbContext.Registrations.Update(registration);
                _dbContext.SaveChanges();
                return _dbContext.Registrations
                    .Include(r => r.Training)
                    .ThenInclude(t => t.Center)
                    .Include(r => r.Training)
                    .ThenInclude(t => t.Location)
                    .FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool isRegistrationExist(string RegistrationNo)
        {
            try
            {
                var registration = _dbContext.Registrations.Where(r => r.RegistrationNo == RegistrationNo);
                if (registration != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ComtrustRegistrationResponse RegisterPayment(string RegistrationNo, string returnUrl)
        {
            try
            {
                Transaction pay = new Transaction(true);
                pay.Initialize("Registration", Convert.ToString(_config["Registration"]));
                pay.Customer = Convert.ToString(_config["Customer"]);
                pay.Channel = Convert.ToString(_config["Channel"]);
                pay.Language = CultureInfo.CurrentCulture.Name;
                pay.SetProperty("Amount", _config["Amount"]);
                pay.SetProperty("Currency", Convert.ToString(_config["Currency"]));
                pay.SetProperty("OrderName", Convert.ToString(_config["OrderName"]));
                pay.SetProperty("TransactionHint", Convert.ToString(_config["TransactionHint"]));
                pay.SetProperty("ReturnPath", returnUrl);
                pay.SetProperty("OrderInfo", _config["OrderName"]);

                pay.Execute();
                if (!(pay == null) && pay.ResponseCode == 0)
                {
                    return  new ComtrustRegistrationResponse()
                    {
                        Transaction = new RegistrationTransaction()
                        {
                            PaymentPortal = pay.GetProperty("PaymentPortal"),
                            ResponseCode = pay.GetProperty("ResponseCode"),
                            TransactionID = pay.GetProperty("TransactionID")
                        }
                    };
                }
				throw new Exception(pay.ResponseDescription);
                //            ComtrustRegistrationRequest request = new ComtrustRegistrationRequest()
                //{
                //	Registration = new ComtrustRegistration()
                //	{
                //		ReturnPath = returnUrl,
                //		Currency = _config["Currency"],
                //		TransactionHint = _config["TransactionHint"],
                //		OrderID = RegistrationNo,
                //		Store = _config["Store"],
                //		Terminal = _config["Terminal"],
                //		Channel = _config["Channel"],
                //		Amount = _config["Amount"],
                //		Customer = _config["Customer"],
                //		OrderName = _config["OrderName"],
                //		UserName = _config["ComtrustUserName"],
                //		Password = _config["Password"],
                //		ExtraData = new ExtraData() { MerchantDescriptor = _config["Customer"], TransactionDescriptor = "Payment for the Training " + RegistrationNo }
                //	}
                //};
                //using (var client = new HttpClient())
                //{
                //	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //	using (var response = await client.PostAsync(_config["ComtrustUrl"],
                //new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")))
                //	{
                //		if (response.IsSuccessStatusCode)
                //		{
                //			var responseAsString = await response.Content.ReadAsStringAsync();
                //			return JsonConvert.DeserializeObject<ComtrustRegistrationResponse>(responseAsString);
                //		}
                //		else
                //		{
                //			var responseAsString = await response.Content.ReadAsStringAsync();
                //			ComtrustRegistrationResponse comtrustRegistrationResponse = JsonConvert.DeserializeObject<ComtrustRegistrationResponse>(responseAsString);
                //			throw new Exception(comtrustRegistrationResponse.Transaction.ResponseDescription);
                //		}
                //	};
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ComtrustFinalizationResponse FinalizePayment(string RegistrationNo, string TransactionID)
        {
            try
            {
                Transaction pay = new Transaction(true);
                pay.Initialize("Finalization", Convert.ToString(_config["Finalization"]));
                pay.Customer = Convert.ToString(_config["Customer"]);
                pay.SetProperty("TransactionID", TransactionID);
                pay.Execute();
                return new ComtrustFinalizationResponse() { 
                    Transaction = new ResponseTransaction()
                    {
                        ResponseCode = Convert.ToString(pay.ResponseCode)
                    }
                };
                //ComtrustFinalizationRequest request = new ComtrustFinalizationRequest()
                //{
                //    Finalization = new Finalization()
                //    {
                //        TransactionID = TransactionID,
                //        Customer = _config["Customer"],
                //        UserName = _config["ComtrustUserName"],
                //        Password = _config["Password"],
                //    }
                //};
                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //    using (var response = await client.PostAsync(_config["ComtrustUrl"],
                //new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")))
                //    {
                //        if (response.IsSuccessStatusCode)
                //        {
                //            var responseAsString = await response.Content.ReadAsStringAsync();
                //            return JsonConvert.DeserializeObject<ComtrustFinalizationResponse>(responseAsString);
                //        }
                //        else
                //        {
                //            var responseAsString = await response.Content.ReadAsStringAsync();
                //            ComtrustFinalizationResponse comtrustFinalizationResponse = JsonConvert.DeserializeObject<ComtrustFinalizationResponse>(responseAsString);
                //            throw new Exception(comtrustFinalizationResponse.Transaction.ResponseDescription);
                //        }
                //    };
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Training GetTrainingDetailsById(int id)
        {
            try
            {
                return _dbContext.Trainings.Where(t => t.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

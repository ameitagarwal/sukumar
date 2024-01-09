using Azure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;
using TMS_Traning_Management.Security;
using TMS_Traning_Management.Services;
using TMS_Traning_Management.ViewModels;

namespace TMS_Traning_Management.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IStringLocalizer<PaymentController> _stringLocalizer;
        private readonly IPaymentService _paymentService;
        private readonly ICommonService _commonService;
        IRegistrationService _registrationService;
        private readonly IDataProtector protector;
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _config;
        public PaymentController(
            IStringLocalizer<PaymentController> stringLocalizer,
            IPaymentService paymentService,
            ICommonService commonService,
            IRegistrationService registrationService,
            ILogger<PaymentController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings,
            IConfiguration config)
        {
            _stringLocalizer = stringLocalizer;
            _logger = logger;
            _paymentService = paymentService;
            _commonService = commonService;
            _registrationService = registrationService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.RegistrationNoRoutValue);
            _config = config;
        }
        public IActionResult Index(string id)
        {
            var decryptedId = protector.Unprotect(id);
            PaymentViewModel model = new PaymentViewModel();

            try
            {
                var reg = _paymentService.GetRegistrationDetailsById(Convert.ToInt32(decryptedId));
                model.Id = reg.Id;
                model.EncryptedId = protector.Protect(reg.Id.ToString());
                model.RegistrationNo = reg.RegistrationNo;
                model.Entity = reg.Entity;
                model.EntityName = reg.EntityName;
                model.DriverName = reg.DriverName;
                model.EmiratesID = reg.EmiratesID;
                model.Email = reg.Email;
                model.Mobile = reg.Mobile;
                model.PaymentStatus = reg.PaymentStatus;
                model.TransactionID = reg.TransactionID;
                model.LocationId = reg.Training.LocationId ?? 0;
                model.CenterId = reg.Training.CenterId ?? 0;
                model.TrainingId = reg.TrainingId;
                model.Training = reg.Training;
                model.Locations = _registrationService.GetLocations();
                model.Centers = _registrationService.GetCentersByLocation(model.LocationId);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            string host = Request.Headers["Referer"].ToString();
            ViewData["BaseUrl"] = host;
            return View(model);
        }

        [HttpPost]
        public IActionResult RegisterPayment(int Id, int TrainingId)
        {
            Registration reg = new Registration();
            var EncryptedId = protector.Protect(Id.ToString());
            try
            {
                //var decryptedId = protector.Unprotect(id);
                reg = _paymentService.GetRegistrationDetailsById(Id);
                if (reg.PaymentStatus == "N")
                {
                    if (reg.TrainingId != TrainingId)
                    {
                        reg = _paymentService.ChangeTraining(Id, TrainingId);
                    }
                    if (reg.Training.AvailableSeats > 0)
                    {
                        reg = _paymentService.UpdateRegistrationDetails(reg.Id, "I", "");
                        reg.EncryptedId = protector.Protect(reg.Id.ToString());
                        ComtrustRegistrationResponse comtrustRegistrationResponse =  _paymentService.RegisterPayment(reg.RegistrationNo, Request.Headers["Referer"].ToString().Replace("Payment/Index", "Payment/FinalizePayment"));
                        if (comtrustRegistrationResponse.Transaction.ResponseCode == "0")
                        {
                            _paymentService.UpdateRegistrationDetails(reg.Id, "I", comtrustRegistrationResponse.Transaction.TransactionID);
                            return Redirect(comtrustRegistrationResponse.Transaction.PaymentPortal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return RedirectToAction("Index", new { id = EncryptedId });
        }

        public IActionResult FinalizePayment(string id)
        {
            var decryptedId = protector.Unprotect(id);
            Registration reg = new Registration();

            try
            {
                reg = _paymentService.GetRegistrationDetailsById(Convert.ToInt32(decryptedId));
                reg.EncryptedId = protector.Protect(reg.Id.ToString());
                if (reg.PaymentStatus == "I" && reg.TransactionID != null)
                {
                    ComtrustFinalizationResponse response = _paymentService.FinalizePayment(reg.RegistrationNo, reg.TransactionID);
                    if (response.Transaction.ResponseCode == "0")
                    {
                        reg = _paymentService.UpdateRegistrationDetails(reg.Id, "C", null);
                        if (!string.IsNullOrEmpty(reg.Email))
                        {
                            string Body = _stringLocalizer["Payment Completion Email Body"].Value.Replace("{RegNo}", reg.RegistrationNo);
                            Body = Body.Replace("{TransactionID}", reg.TransactionID);
							Body = Body.Replace("{Amount}", "AED " + _config["Amount"]);
							Body = Body.Replace("{City}", reg.Training.Location.TitleEn);
                            Body = Body.Replace("{Location}", reg.Training.Center.TitleEn);
                            Body = Body.Replace("{Link}", _stringLocalizer[reg.Training.Center.Url]);
                            Body = Body.Replace("{StartDate}", reg.Training.StartDateTime.ToString("ddd, MMM dd, yyyy"));
                            Body = Body.Replace("{EndDate}", reg.Training.EndDateTime.ToString("ddd, MMM dd, yyyy"));

                            string paymentlink = Request.Headers["Origin"].ToString();

                            //Body = Body.Replace("{paymentlink}", paymentlink + "/en/Payment/Index/" + reg.EncryptedId);

                            var resulrt = _commonService.SendMail(reg.Email, _stringLocalizer["Payment Completion Email Subject"].Value.Replace("{RegNo}", reg.RegistrationNo), Body);
                            if (resulrt != "Success")
                            {
                                _logger.Log(LogLevel.Error, resulrt);
                            }
                        }
                        var Message = _stringLocalizer["Payment Completion SMS"].Value.Replace("{Amount}", "AED" + _config["Amount"]);
                        Message = Message.Replace("{TransactionID}", reg.TransactionID);
                        Message = Message.Replace("{Location}", reg.Training.Center.TitleEn);
                        Message = Message.Replace("{TrainingDate}", reg.Training.StartDateTime.ToString("ddd, MMM dd, yyyy"));
                        _commonService.SendSMS(Message, reg.Mobile);
                    }
                    else
                    {
                        _paymentService.UpdateRegistrationDetails(reg.Id, "N", null);
                        TempData["PaymentError"] = response.Transaction.ResponseDescription;
                        return RedirectToAction("Index", new { id = id });
                    }
                }
            }
            catch (Exception ex)
            {
                _paymentService.UpdateRegistrationDetails(reg.Id, "N", "");
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return View(reg);
        }

        [HttpPost, ActionName("GetTrainingDetailsById")]
        public JsonResult GetTrainingDetailsById(int id)
        {
            Training training = new Training();
            try
            {
                training = _paymentService.GetTrainingDetailsById(id);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return Json(training);
        }
        [HttpPost, ActionName("GetTrainingsByLocation")]
        public JsonResult GetTrainingsByLocation(int LocationId, int CenterId)
        {
            List<Training> training = new List<Training>();
            try
            {
                training = _paymentService.GetTrainings(LocationId, CenterId);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return Json(training);
        }
    }
}

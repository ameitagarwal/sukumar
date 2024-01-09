using System.Reflection.Metadata;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;
using TMS_Traning_Management.Security;
using TMS_Traning_Management.Services;
using TMS_Traning_Management.ViewModels;
using Document = iTextSharp.text.Document;

namespace TMS_Traning_Management.Controllers
{
	public class RegistrationController : Controller
	{
		private const int PAGE_SIZE = 10;

		private readonly IStringLocalizer<RegistrationController> _stringLocalizer;
		private readonly IRegistrationService _registrationService;
		private readonly ICommonService _commonService;
		private readonly IConfiguration _config;
		private readonly ILogger<RegistrationController> _logger;
		private readonly IDataProtector protector;
		public RegistrationController(
			IStringLocalizer<RegistrationController> stringLocalizer,
			IRegistrationService registrationService,
			ICommonService commonService,
			IConfiguration config,
			ILogger<RegistrationController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
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
			RegistrationViewModel model = new RegistrationViewModel();
			try
			{
				model = new RegistrationViewModel()
				{
					DriverName = "",
					Email = "",
					Mobile = "",
					EmiratesID = "",
					Locations = _registrationService.GetLocations(),
					Entities = _registrationService.GetEntities(),
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

		[HttpPost]
		public IActionResult Index(RegistrationViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Registration registration = _registrationService.GetRegistrationDetailsByEmiratesId(model.EmiratesID, model.TrainingId);
					if (registration == null)
					{
						Registration Item = new Registration()
						{
							Entity = model.Entity,
							EntityName = model.EntityName,
							DriverName = model.DriverName,
							Email = model.Email,
							Mobile = model.Mobile,
							EmiratesID = model.EmiratesID,
							PaymentStatus = "N",
							TrainingId = model.TrainingId,
							Created = DateTime.Now,
							Updated = DateTime.Now,
						};
						registration = _registrationService.Create(Item);
						registration.EncryptedId = protector.Protect(registration.Id.ToString());
						if (!string.IsNullOrEmpty(model.Email))
						{
							string Body = _stringLocalizer["Registration Completion Email Body"].Value.Replace("{RegNo}", registration.RegistrationNo);
							Body = Body.Replace("{City}", registration.Training.Location.TitleEn);
							Body = Body.Replace("{Location}", registration.Training.Center.TitleEn);
							Body = Body.Replace("{Link}", _stringLocalizer[registration.Training.Location.TitleEn]);
							Body = Body.Replace("{StartDate}", registration.Training.StartDateTime.ToString("ddd, MMM dd, yyyy"));
							Body = Body.Replace("{EndDate}", registration.Training.EndDateTime.ToString("ddd, MMM dd, yyyy"));

							string paymentlink = Request.Headers["Origin"].ToString();

							Body = Body.Replace("{paymentlink}", paymentlink + "/en/Payment/Index/" + registration.EncryptedId);

							var resulrt = _commonService.SendMail(registration.Email, _stringLocalizer["Registration Completion Email Subject"].Value, Body);
							if(resulrt != "Success")
							{
                                _logger.Log(LogLevel.Error, resulrt);
                            }
						}
                        var Message = _stringLocalizer["Registration Completion SMS"].Value.Replace("{RegistrationNo}", registration.RegistrationNo);
                        Message = Message.Replace("{Loc}", registration.Training.Center.TitleEn);
                        Message = Message.Replace("{TD}", registration.Training.StartDateTime.ToString("ddd, MMM dd, yyyy"));

                        _commonService.SendSMS(Message, registration.Mobile);
						return RedirectToAction("Complete", new { id = registration.EncryptedId });
					}
					else
					{
						registration.EncryptedId = protector.Protect(registration.Id.ToString());
						var link = Request.Headers["Referer"].ToString().ToLower().Replace("/registration", "/Payment/Index/") + registration.EncryptedId;
						TempData["Duplicate Registration Message"] = _stringLocalizer["Duplicate Registration Message"].Value.Replace("{clickhere}", link);
					}
				}
				model.Locations = _registrationService.GetLocations();
				model.Entities = _registrationService.GetEntities();
				model.Location = 0;
				//model.Centers = _registrationService.GetCentersByLocation(model.Location);
				//model.Trainings = _registrationService.GetTrainings(Convert.ToInt32(model.Location), Convert.ToInt32(model.Center));
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return View(model);
		}


        public IActionResult MyRegistration(string EmiratesId)
        {
            EmiratesId = "784-0000-0000000-0";
            List<MyRegistration> registrations = new List<MyRegistration>();
			try
            {
                registrations = _registrationService.GetMyRegistrationDetailsByEmiratesId(EmiratesId);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return View(registrations);
        }


		//Implement for Paging
        //public IActionResult MyRegistration(string EmiratesId)
        //{
        //    EmiratesId = "784-0000-0000000-0";
        //    List<MyRegistration> registrations = new List<MyRegistration>();
        //    var modelresult = new ShowPaging();
        //    try
        //    {
        //        modelresult.DisplayResult = _registrationService.GetMyRegistrationDetailsByEmiratesId(EmiratesId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(LogLevel.Error, ex.Message);
        //    }
        //    return View(modelresult);
        //}
        //public IActionResult ShowPaging(ShowPaging model,
        //					 int page = 1, int inputNumber = 1)
        //{
        //	if (ModelState.IsValid)
        //	{
        //		var displayResult = new List<MyRegistration>();
        //              model.EmiratesID = "784-0000-0000000-0";
        //              model.DisplayResult =  _registrationService.GetMyRegistrationDetailsByEmiratesId(model.EmiratesID);
        //              string message;

        //		//set model.pageinfo
        //		model.PageInfo = new PageInfo();
        //		model.PageInfo.CurrentPage = page;
        //		model.PageInfo.ItemsPerPage = PAGE_SIZE;
        //		model.PageInfo.TotalItems = inputNumber;

        //		//Set model.displayresult - numbers list
        //		for (int count = model.PageInfo.PageStart;
        //				 count <= model.PageInfo.PageEnd; count++)
        //		{
        //			message = count.ToString();
        //			displayResult.Add(message.Trim());
        //		}
        //		model.DisplayResult = displayResult;
        //	}
        //	//return view model
        //	return View(model);
        //}

        [HttpPost, ActionName("GetTrainingsByLocation")]
		public JsonResult GetTrainingsByLocation(int LocationId, int CenterId)
		{
			List<Training> training = new List<Training>();
			try
			{
                training = _registrationService.GetTrainings(LocationId, CenterId);
            }
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return Json(training);
		}

		[HttpPost, ActionName("GetCentersByLocation")]
		public JsonResult GetCentersByLocation(int LocationId)
		{
			List<Center> centers = new List<Center>();
			try
			{
				centers = _registrationService.GetCentersByLocation(LocationId);
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return Json(centers);
		}
		[HttpPost, ActionName("GetCenterById")]
		public JsonResult GetCenterById(int Id)
		{
			Center center = new Center();
			try
			{
				center = _registrationService.GetCenterDetails(Id);
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return Json(center);
		}
		[HttpGet]
		public IActionResult Complete(string id)
		{
			Registration registration = new Registration();
			try
			{
				var decryptedId = protector.Unprotect(id);
				registration = _registrationService.GetRegistrationDetailsById(Convert.ToInt32(decryptedId));
				registration.EncryptedId = protector.Protect(registration.Id.ToString());
			}
			catch (Exception ex)
			{
                _logger.Log(LogLevel.Error, ex.Message);
            }
			return View(registration);
		}
	}
}

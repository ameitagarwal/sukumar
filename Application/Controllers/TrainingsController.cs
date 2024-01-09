using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;
using TMS_Traning_Management.Security;
using TMS_Traning_Management.Services;
using TMS_Traning_Management.ViewModels;

namespace TMS_Traning_Management.Controllers
{
	public class TrainingsController : Controller
	{
		private readonly IStringLocalizer<TrainingsController> _stringLocalizer;
		private readonly ITrainingsService _trainingsService;
		private readonly ICommonService _commonService;
		private readonly IConfiguration _config;
		private readonly ILogger<TrainingsController> _logger;
		private readonly IDataProtector protector;
		public TrainingsController(
			IStringLocalizer<TrainingsController> stringLocalizer,
			ITrainingsService trainingsService,
			ICommonService commonService,
			IConfiguration config,
			ILogger<TrainingsController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_stringLocalizer = stringLocalizer;
			_trainingsService = trainingsService;
			_commonService = commonService;
			_config = config;
			_logger = logger;
			protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.RegistrationNoRoutValue);
		}
		[HttpGet]
		public IActionResult Index()
		{
			List<Training> trainings = new List<Training>();
			try
			{
				TrainingsViewModel model = new TrainingsViewModel();
				model = new TrainingsViewModel()
				{
					AvailableSeats = 0,
					TotalSeats = 0,
					Locations = _trainingsService.GetLocations(),
					Centers = _trainingsService.GetCenters(),
				};				

				foreach(var item in _trainingsService.GetTrainingsDetails())
				{
					trainings.Add(new Training()
					{
						AvailableSeats = item.AvailableSeats,
						TotalSeats = item.TotalSeats,
						CenterId = item.CenterId,
						Center = item.Center,
						Location = item.Location,
						LocationId = item.LocationId,
						StartDateTime = item.StartDateTime,
						EndDateTime = item.EndDateTime,
						RegistrationNo = item.RegistrationNo,
						EncryptedId = protector.Protect(item.Id.ToString())
					});
				}

			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}
			return View(trainings);
		}
		
		public PartialViewResult Edit(string id)
		{
			TrainingsViewModel trainings = new TrainingsViewModel();
			if (id != "undefined")
			{
				var decrypted = protector.Unprotect(id);
				trainings = new TrainingsViewModel()
				{
					Locations = _trainingsService.GetLocations(),
					Centers = _trainingsService.GetCenters(),
					Training = _trainingsService.GetTrainingsDetailsById(Convert.ToInt32(decrypted))
				};
			}

			return PartialView("_EditTraining", trainings);
        }
		[HttpPost]
        public IActionResult Edit(TrainingsViewModel trainingsViewModel)
        {
			try
			{
				_trainingsService.Update(trainingsViewModel.Training);
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, ex.Message);
			}

            return RedirectToAction("Index");
        }

		[HttpGet]
		public IActionResult Delete(string id)
		{
			Training trainings = new Training();
			var decrypted = protector.Unprotect(id);
			var empfromdb = _trainingsService.GetTrainingsDetailsById(Convert.ToInt32(decrypted));

			try
			{
				if (empfromdb == null)
				{
					return NotFound();
				}
			}
			catch (Exception ex)
			{
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return PartialView("_DeleteTraining", empfromdb);
        }
		[HttpPost]
		public IActionResult Delete(Training training)
		{
            Training trainings = new Training();
			try
			{
				var empfromdb = _trainingsService.Delete(training);
			}
			catch (Exception ex)
			{
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return PartialView("~/Views/Trainings/_DeleteTraining.cshtml", trainings);
        }
		[HttpGet]
        public IActionResult Registration(string id)
        {
			TrainingsViewModel trainings = new TrainingsViewModel();
			try
			{
				if (id != "undefined")
				{
                    var decrypted = protector.Unprotect(id);
                    trainings = new TrainingsViewModel()
					{
						Locations = _trainingsService.GetLocations(),
						Centers = _trainingsService.GetCenters(),
						Training = _trainingsService.GetTrainingsDetailsById(Convert.ToInt32(decrypted)),
						Registration = _trainingsService.GetRegistrationDetailsByTrainingId(Convert.ToInt32(decrypted))
					};
				}
			}
			catch (Exception ex)
			{
                _logger.Log(LogLevel.Error, ex.Message);
            }
			return View(trainings);
        }
		[HttpPost]
		public IActionResult Registration(TrainingsViewModel regs)
		{
			return View();
		}


	}
}

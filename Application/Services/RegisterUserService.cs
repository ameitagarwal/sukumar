using Microsoft.EntityFrameworkCore;
using TMS_Traning_Management.Controllers;
using TMS_Traning_Management.Interfaces;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Services
{
	public class RegisterUserService : IRegisterUserService
	{
		private readonly AppDbContext _dbContext;
		private readonly IConfiguration _config;
		//private readonly ILogger<DriverRegistrationController> _logger;

		public RegisterUserService(AppDbContext dbContext, IConfiguration config)
		{
			_dbContext = dbContext;
			_config = config;
		}
		public UserOTP Update(UserOTP updatedModel)
		{
			try
			{
				var existingModel = _dbContext.UserOTP.FirstOrDefault(r => r.Id == updatedModel.Id);

				if (existingModel != null)
				{
					// Update fields of the existing entity
					existingModel.OTP = updatedModel.OTP;
					existingModel.Status = updatedModel.Status;
					// Update other fields as needed

					_dbContext.SaveChanges();
				}

				return existingModel;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public UserOTP Create(UserOTP model)
		{
			try
			{
				_dbContext.UserOTP.Add(model);
				_dbContext.SaveChanges();

				model = _dbContext.UserOTP
					//.Include(r => r.OTP)
					//.Include(t => t.Status)
					//.Include(r => r.EmiratesID)
					//.Include(t => t.UserId)
					.FirstOrDefault(r => r.Id == model.Id);
				return model;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Country> GetCountry()
		{
			try
			{
				return _dbContext.Country.ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<SecurityQuestion> GetSecurityQuestion()
		{
			try
			{
				return _dbContext.SecurityQuestion.ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public UserOTP GetOTPByEmiratesID(string EmiratesID)
		{
			try
			{
				return _dbContext.UserOTP.Where(r => r.EmiratesID == EmiratesID).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}

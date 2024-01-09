using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using TMS_Training_Management_StatusChange_Scheduler.Interfaces;

namespace TMS_Training_Management_StatusChange_Scheduler.Services
{
	public class RegistrationService : IRegistrationService
	{
		private readonly IConfiguration _config;
		private readonly ILogger<RegistrationService> _logger;
		private AppDbContext _dbContext;
		public RegistrationService(IConfiguration config, ILogger<RegistrationService> logger)
		{
			_config = config;
			_logger = logger;
		}
		private DbContextOptions<AppDbContext> GetAllOptions()
		{
			DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

			optionsBuilder.UseSqlServer(_config["ConnectionString"].ToString());

			return optionsBuilder.Options;
		}
		public string ResetpaymentStatus()
		{
			using (_dbContext = new AppDbContext(GetAllOptions()))
			{
				try
				{
					_logger.Log(LogLevel.Information, "Started getting Registrations from Database.");
					List<Registration> registrations = _dbContext.Registrations.Where(r => r.PaymentStatus == "I" && r.TTC < DateTime.Now).Include(r => r.Training).ToList();
					for (var i = 0; i < registrations.Count; i++)
					{
						Registration registration = registrations[i];
						_logger.Log(LogLevel.Information, "Started updating the Payment Status  for the Registration - " + registration.RegistrationNo);
						_logger.Log(LogLevel.Information, "No Of seats before update - " + registration.RegistrationNo + " - " + registration.Training.AvailableSeats);
						registration.PaymentStatus = "N";
						registration.Updated = DateTime.Now;
						registration.Training.AvailableSeats = registration.Training.AvailableSeats + 1;
						_dbContext.Update(registration);
						_dbContext.SaveChanges();
						_logger.Log(LogLevel.Information, "No Of seats after update - " + registration.RegistrationNo + " - " + registration.Training.AvailableSeats);
						_logger.Log(LogLevel.Information, "Completed updating the Payment Status  for the Registration - " + registration.RegistrationNo);
					}
					_logger.Log(LogLevel.Information, "Completed Status Update.");
					return "Success";
				}
				catch (Exception ex)
				{
					MailMessage msg = new MailMessage();
					SmtpClient smtp = new SmtpClient(_config["SMTPServerIP"]);
					smtp.Credentials = new System.Net.NetworkCredential(_config["FromAddress"], _config["SMTPServerPassword"]);
					smtp.Port = Convert.ToInt32(_config["SMTPServerPort"]);

					msg.From = new MailAddress(_config["FromAddress"]);
					msg.IsBodyHtml = true;
					msg.To.Add(_config["ToAddress"]);
					msg.Subject = "TMS Training management Scheduler Error Occured!";
					msg.Body = ex.Message;
					smtp.Send(msg);
					throw ex;
				}
			}
		}
	}
}
